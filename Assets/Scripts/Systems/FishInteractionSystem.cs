using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class FishInteractionSystem : SystemBase
{
    // divide fishes to chunks
    public NativeMultiHashMap<int, Boid_ComponentData> cellVsEntityPositions;
    private int3[] lazyChecks;
    private NativeArray<int3> ints;

    public static int GetUniqueKeyForPosition(float3 position, int cellSize)
    {
        return (int)((17 * math.floor(position.x / cellSize)) + (19 * math.floor(position.y / cellSize)) + (23 * math.floor(position.z / cellSize)));
    }

    protected override void OnCreate()
    {
        cellVsEntityPositions = new NativeMultiHashMap<int, Boid_ComponentData>(0, Allocator.Persistent);
        
        // lazy checks
        lazyChecks = new int3[] {
            new int3(0, 0, 0),
            new int3(1, 0, 0),
            new int3(-1, 0, 0),
            new int3(0, 1, 0),
            new int3(0, -1, 0),
            new int3(0, 0, 1),
            new int3(0, 0, -1)
        };

        ints = new NativeArray<int3>(lazyChecks, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        if (CamControlScr.Instance.isPaused) return;

        EntityQuery fishes = GetEntityQuery(typeof(Boid_ComponentData));
        cellVsEntityPositions.Clear();
        if (fishes.CalculateEntityCount() > cellVsEntityPositions.Capacity)
        {
            cellVsEntityPositions.Capacity = fishes.CalculateEntityCount();
        }

        // get static variables
        int cellSize = MasterScript.Instance.cellSize;

        // fill the hashmap for the positions of the fishes
        NativeMultiHashMap<int, Boid_ComponentData>.ParallelWriter cellVsEntityPositionsParallel = cellVsEntityPositions.AsParallelWriter();
        Entities.ForEach((ref Boid_ComponentData boidData, ref Translation trans) =>
        {
            Boid_ComponentData bcValues = boidData;
            bcValues.currentPosition = trans.Value;
            //cellVsEntityPositionsParallel.Add(GetUniqueKeyForPosition(trans.Value, boidData.cellSize), bcValues);
            cellVsEntityPositionsParallel.Add(GetUniqueKeyForPosition(trans.Value, cellSize), bcValues);

        }).ScheduleParallel();


        // check boids size change
        if (MasterScript.Instance.updateSize)
        {
            float newSize = MasterScript.Instance.boidScale;
            MasterScript.Instance.updateSize = false;
            Entities.ForEach((ref Boid_ComponentData boidData, ref Scale scale) =>
            {
                scale.Value = newSize;
            }).ScheduleParallel();
        }

        // get static variable v2
        float perceptionRadius = MasterScript.Instance.perceptionRad;
        float separationBias = MasterScript.Instance.separationBias;
        float alignmentBias = MasterScript.Instance.alignmentBias;
        float cohesionBias = MasterScript.Instance.cohesionBias;
        float speed = MasterScript.Instance.maxSpeed;
        float step = MasterScript.Instance.step;
        float rotStep = MasterScript.Instance.rotStep;
        float targetBias = MasterScript.Instance.targetBias;
        float3 target = MasterScript.Instance.target;
        float boidSep = MasterScript.Instance.boidSep;
        float deltaTime = Time.DeltaTime;

        // change the velocity and positions of the fish based on neighbor fish

        NativeMultiHashMap<int, Boid_ComponentData> cellVsEntityPositionsForJob = cellVsEntityPositions;

        NativeArray<int3> intsForJob = ints;

        Entities.WithBurst().WithReadOnly(cellVsEntityPositionsForJob).WithReadOnly(intsForJob).ForEach((ref Boid_ComponentData boidData, ref Translation trans, ref Rotation rot) =>
        {
            //int key = GetUniqueKeyForPosition(trans.Value, boidData.cellSize);
            int total = 0;
            float hueSimTotal = 0f;
            float3 seperation = float3.zero;
            float3 alignment = float3.zero;
            float3 coheshion = float3.zero;

            Boid_ComponentData neighbour;
            NativeMultiHashMapIterator<int> fishIteratorForPosition;

            foreach (int3 i in intsForJob)
            {

                int key = GetUniqueKeyForPosition(trans.Value + i * cellSize, cellSize);

                if (cellVsEntityPositionsForJob.TryGetFirstValue(key, out neighbour, out fishIteratorForPosition))
                {
                    do
                    {
                        float distSquared = twoPointsDistSquared(trans.Value, neighbour.currentPosition);
                        //if (!trans.Value.Equals(neighbour.currentPosition) && distSquared < boidData.perceptionRadius * boidData.perceptionRadius)
                        if (!trans.Value.Equals(neighbour.currentPosition) && distSquared < perceptionRadius * perceptionRadius)
                        {
                            float hueDif = boidData.hue - neighbour.hue;
                            hueDif = (hueDif < 0f) ? -hueDif : hueDif;
                            if (hueDif > 0.5f) hueDif = 1f - hueDif;
                            float hueSim = boidSep - hueDif * 4;
                            float3 distanceFromTo = trans.Value - neighbour.currentPosition;
                            seperation += distanceFromTo / distSquared;
                            alignment += neighbour.velocity * hueSim;
                            coheshion += neighbour.currentPosition * hueSim;
                            total += 1;
                            hueSimTotal += hueSim;
                        }
                    } while (cellVsEntityPositionsForJob.TryGetNextValue(out neighbour, ref fishIteratorForPosition));
                }
            }

            if (total > 0)
            {
                float totalInv = 1f / total;

                seperation *= totalInv;
                seperation -= boidData.velocity;

                seperation *= separationBias;
                //seperation = math.normalize(seperation) * separationBias;
                //seperation = math.normalize(seperation) * boidData.separationBias;

                alignment *= totalInv;
                alignment -= boidData.velocity;

                alignment *= alignmentBias;
                //alignment = math.normalize(alignment) * alignmentBias;
                //alignment = math.normalize(alignment) * boidData.alignmentBias;

                coheshion -= hueSimTotal * trans.Value;
                coheshion *= totalInv;
                coheshion -= boidData.velocity;

                coheshion *= cohesionBias;
                //coheshion = math.normalize(coheshion) * cohesionBias;
                //coheshion = math.normalize(coheshion) * boidData.cohesionBias;
            }

            boidData.acceleration += seperation + alignment + coheshion;

            boidData.velocity += boidData.acceleration;
            float3 normVel = math.normalize(boidData.velocity);
            boidData.velocity = normVel * speed;
            //boidData.velocity = math.normalize(boidData.velocity) * boidData.speed;

            rot.Value = math.slerp(rot.Value, quaternion.LookRotation(normVel, math.up()), deltaTime * rotStep);
            //rot.Value = math.slerp(rot.Value, quaternion.LookRotation(math.normalize(boidData.velocity), math.up()), deltaTime * boidData.rotStep);
            trans.Value = math.lerp(trans.Value, (trans.Value + boidData.velocity), deltaTime * step);
            //trans.Value = math.lerp(trans.Value, (trans.Value + boidData.velocity), deltaTime * boidData.step);
            boidData.acceleration = math.normalize(target - trans.Value) * targetBias;
            //boidData.acceleration = math.normalize(boidData.target - trans.Value) * boidData.targetBias;
        }).ScheduleParallel();
    }

    protected override void OnDestroy()
    {
        cellVsEntityPositions.Dispose();
        ints.Dispose();
    }

    private static float twoPointsDistSquared(float3 a, float3 b)
    {
        return (a.x - b.x) * (a.x - b.x) +
            (a.y - b.y) * (a.y - b.y) +
            (a.z - b.z) * (a.z - b.z);
    }
}

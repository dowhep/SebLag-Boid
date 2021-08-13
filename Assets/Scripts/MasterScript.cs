using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;

public class MasterScript : MonoBehaviour
{
    public float boidsSpawnRadius;
    public int boidsPerInterval;
    public int boidsToSpawn;
    public float interval;
    public float cohesionBias;
    public float separationBias;
    public float alignmentBias;
    public float targetBias;
    public float perceptionRad;
    public float3 target;
    public Material material;
    public Mesh mesh;
    public float maxSpeed;
    public float step;
    public float rotStep;
    public int cellSize;
    [Range(0f, 2f)] public float boidSep = 1f;
    [HideInInspector] public bool updateSize;
    public float defaultBoidScale;

    public float boidScale { get; private set; }
    public static MasterScript Instance;

    private EntityManager entitymanager;
    private float elapsedTime;
    private int totalSpawnedBoids { get { return entities.Length; } }
    private EntityArchetype BoidArchetype;
    private float3 currentPosition;
    private NativeList<Entity> entities;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        boidScale = defaultBoidScale;
        updateSize = false;
        entities = new NativeList<Entity>(Allocator.Persistent);
        entitymanager = World.DefaultGameObjectInjectionWorld.EntityManager;
        currentPosition = transform.position;
        BoidArchetype = entitymanager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(Boid_ComponentData),
            typeof(Scale),
            typeof(URPMaterialPropertyBaseColor),
            typeof(URPMaterialPropertyEmissionColor)
            );
    }

    private void Update()
    {
        if (CamControlScr.Instance.isPaused) return;


        if (totalSpawnedBoids == boidsToSpawn) return;
        
        else if (totalSpawnedBoids > boidsToSpawn)
        {
            int dif = totalSpawnedBoids - boidsToSpawn;
            NativeArray<Entity> temp = new NativeArray<Entity>(dif, Allocator.Temp);
            for (int i = 0; i < dif; i++)
            {
                temp[i] = entities[entities.Length - 1 - i];
            }
            entitymanager.DestroyEntity(temp);
            entities.RemoveRangeSwapBackWithBeginEnd(entities.Length - dif, entities.Length);
            temp.Dispose();
            return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= interval)
        {
            elapsedTime = 0;
            for (int i = 0; i <= boidsPerInterval; i++)
            {
                if (totalSpawnedBoids == boidsToSpawn)
                {
                    break;
                }
                Entity e = entitymanager.CreateEntity(BoidArchetype);

                entitymanager.AddComponentData(e, new Translation
                {
                    Value = currentPosition + new float3(UnityEngine.Random.insideUnitSphere) * boidsSpawnRadius
                });
                entitymanager.AddComponentData(e, new Scale
                {
                    Value = boidScale
                });

                float hue = UnityEngine.Random.Range(0f, 1f);
                Color color = Color.HSVToRGB(hue, 1f, 1f);
                float4 color4 = new float4(color.r, color.g, color.b, color.a);

                entitymanager.AddComponentData(e, new URPMaterialPropertyBaseColor
                {
                    Value = color4
                });
                
                entitymanager.AddComponentData(e, new URPMaterialPropertyEmissionColor
                {
                    Value = color4
                });

                entitymanager.AddComponentData(e, new Boid_ComponentData
                {
                    velocity = new float3(UnityEngine.Random.onUnitSphere) * maxSpeed,
                    //perceptionRadius = perceptionRad,
                    //speed = maxSpeed,
                    //step = step,
                    //cohesionBias = cohesionBias,
                    //separationBias = separationBias,
                    //alignmentBias = alignmentBias,
                    //target = target,
                    //targetBias = targetBias,
                    //cellSize = cellSize,
                    //rotStep = rotStep,
                    hue = hue
                });


                entitymanager.AddSharedComponentData(e, new RenderMesh
                {
                    mesh = mesh,
                    material = material,
                    castShadows = UnityEngine.Rendering.ShadowCastingMode.On
                });

                entities.Add(e);
            }
        }
    }
    private void OnDestroy()
    {
        entities.Dispose();
    }
    public void changeBoidsSize(float value)
    {
        boidScale = value;
        updateSize = true;
    }
}
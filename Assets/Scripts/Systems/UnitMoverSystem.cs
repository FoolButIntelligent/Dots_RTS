using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };
        unitMoverJob.ScheduleParallel();
        //首先构建主线程代码，跑起来改为 job的多线程代码
        // foreach (var 
        //    (localTransform,
        //    unitMover,
        //    physicsVelocity)
        //          in SystemAPI.Query<
        //              RefRW<LocalTransform>,
        //              RefRO<UnitMover>,
        //              RefRW<PhysicsVelocity>>()) {
        //     
        //     float3 moveDirection = unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;
        //     moveDirection = math.normalize(moveDirection);
        //
        //     localTransform.ValueRW.Rotation = 
        //         math.slerp(localTransform.ValueRO.Rotation, 
        //             quaternion.LookRotation(moveDirection,math.up()), 
        //             unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
        //     
        //     physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;   
        //     //physicsVelocity.ValueRW.Angular = float3.zero;
        //     //localTransform.ValueRW.Position += moveDirection * moveSpeed.ValueRO.value * SystemAPI.Time.DeltaTime;
        // } 
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;
    public void Execute(ref LocalTransform localTransform,in UnitMover unitMover,ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;

        float reachedTargetDistanceSq = 0.1f;
        if (math.lengthsq(moveDirection) < reachedTargetDistanceSq)
        {
            //Reached the target position & stop moving
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
        
        moveDirection = math.normalize(moveDirection);

        localTransform.Rotation = 
            math.slerp(localTransform.Rotation, 
                quaternion.LookRotation(moveDirection,math.up()), 
                unitMover.rotationSpeed * deltaTime);
            
        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;    
    }
}

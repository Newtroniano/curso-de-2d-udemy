namespace GlobalTypes
{
    public enum GroundTypes
    {
        None,
        LevelGeometry,
        OneWayPlatform,
        MovingPlatform,
        CollapsablePlatform,
        JumpPad
    }


    public enum WallType
    {
        None,
        Normal, 
        Stick
    }


    public enum AirEffectorType
    {
        None, 
        Ladder,
        Updraft,
        TractorBeam
    }

    public enum ControllerMoveType
    {
        physicBased,
        nonPhysicsBased,
        none
    }

    public enum BossType
    {
        Normal,
        None,
        Boss1,
        Boss2,
        Boss3,
        Boss4,
        Boos5,
        Boss6,
        Boss7,
        Boss8
    }


    public enum ShootType
    {
        Normal,
        shoot1,
        shoot2,
        shoot3,
        shoot4,
        shoot5,
        shoot6,
        shoot7,
        shoot8,
        None
    }


    public enum HitBoxType
    {
        Normal,
        HitBox,
        HurtBox,
        None
    }

}
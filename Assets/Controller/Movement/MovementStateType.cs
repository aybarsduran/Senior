namespace IdenticalStudios
{
    public enum MovementStateType
    {
        None = -1,
        Idle = 0,
        Walk = 10,
        Run = 20,
        Slide = 30,
        Crouch = 40,
        Jump = 50,
        Airborne = 60,
        LadderClimb = 70, // Not Implemented
        // Mantle = 80,      // Not Implemented
        // Swim = 90,        // Not Implemented
        
        // Here you can add more state types.
    }
}
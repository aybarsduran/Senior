namespace IdenticalStudios.ProceduralMotion
{

    // The ease type defines the type ofthe Ease that will be used during the  animation. the ease type can be changed using SetTween or one of the SetTween methods.
    public enum EaseType
    {
         
        //the tween will ease using a linear motion.        
        Linear = 0,

         
        //the tween will ease using a sine in motion.
        SineIn = 10,

         
       // the tween will ease using a sine out motion.       
        SineOut = 11,

         
       // the tween will ease using a sine in out motion.       
        SineInOut = 12,

         
       // the tween will ease using a quad in motion.       
        QuadIn = 20,

         
       // the tween will ease using a quad out motion.        
        QuadOut = 21,

         
       // the tween will ease using a quad in out motion.         
        QuadInOut = 22,

         
        //the tween will ease using a cubic in motion.        
        CubicIn = 30,

         
        //the tween will ease using a cubic out motion.         
        CubicOut = 31,

         
        //the tween will ease using a cubic in out motion.         
        CubicInOut = 32,

         
        //the tween will ease using a quart in motion.         
        QuartIn = 40,

         
        //the tween will ease using a quart out motion.         
        QuartOut = 41,

         
        //the tween will ease using a quart in out motion.         
        QuartInOut = 42,

         
        //the tween will ease using a quint in motion.         
        QuintIn = 50,

         
        //the tween will ease using a quint out motion.         
        QuintOut = 51,

         
        //the tween will ease using a quint in out motion.         
        QuintInOut = 52,

         
        //the tween will ease using a expo in motion.         
        ExpoIn = 60,

         
        //the tween will ease using a expo out motion.        
        ExpoOut = 61,

         
        //the tween will ease using a expo in out motion.         
        ExpoInOut = 62,

         
        //the tween will ease using a circ in motion.        
        CircIn = 70,

         
        //the tween will ease using a circ out motion.
         
        CircOut = 71,

         
        //the tween will ease using a circ in out motion.        
        CircInOut = 72,

         
        //the tween will ease using a back in motion.        
        BackIn = 80,

         
        //the tween will ease using a back out motion.        
        BackOut = 81,

         
        //the tween will ease using a back in out motion.       
        BackInOut = 82,

         
        //the tween will ease using a elastic in motion.         
        ElasticIn = 90,

         
        //the tween will ease using a elastic out motion.        
        ElasticOut = 91,

         
        //the tween will ease using a elastic in out motion.         
        ElasticInOut = 92,

         
        //the tween will ease using a bounce in motion.         
        BounceIn = 100,

         
        //the tween will ease using a bounce out motion.         
        BounceOut = 101,

       
        //the tween will ease using a bounce in out motion.
        BounceInOut = 102,
    }
}
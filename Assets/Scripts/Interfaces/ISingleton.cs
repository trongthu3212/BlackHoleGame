namespace BlackHole.Interfaces
{
    public interface ISingleton<out T> where T : class
    {
        public static T Instance { get; }
    }
}
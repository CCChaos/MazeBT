

namespace CCCommon
{
    public interface ISingleton
    {
        //init
        void InitSingleton();
        // clear
        void ReleaseSingleton();
        // update
        void OnUpdate();
    }
}
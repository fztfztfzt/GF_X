
using NavMeshPlus.Components;
using UnityGameFramework.Runtime;

public class NavigationComponent : GameFrameworkComponent
{
    public NavMeshSurface Surface2D;
    private void Update()
    {
        Surface2D.UpdateNavMesh(Surface2D.navMeshData);
    }
}

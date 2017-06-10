using System.IO;
using MetroOverhaul.NEXT;
using MetroOverhaulModPatch.RedirectionFramework.Attributes;
using ObjUnity3D;
using UnityEngine;

namespace MOMPatch.Detours
{
    [TargetType(typeof(AssetManager))]
    public class AssetManagerDetour
    {
        [RedirectMethod]
        private static Mesh LoadMesh(string fullPath, string meshName)
        {
            var mesh = new Mesh();
            using (var fileStream = File.Open(fullPath, FileMode.Open))
            {
                mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
            }
            var name = Path.GetFileNameWithoutExtension(meshName);
            mesh.name = name;
            //if (!name.Contains("LOD"))
            //{
            //    mesh.UploadMeshData(true);
            //}
            return mesh;
        }
    }
}
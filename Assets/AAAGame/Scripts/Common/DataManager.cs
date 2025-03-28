using dnlib.DotNet.MD;
using Luban;
using System.IO;
using UnityEngine;
namespace cfg
{
    public partial class Tables
    {
        private static Tables m_Instance;
        public static Tables Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new cfg.Tables(LoadByteBuf);
                }
                return m_Instance;
            }
        }

        private static ByteBuf LoadByteBuf(string file)
        {
            if (!Application.isPlaying)
            {
                return new ByteBuf(File.ReadAllBytes($"{Application.dataPath}/AAAGame/GenerateData/bytes/{file}.bytes"));
            }

            return new ByteBuf(GF.Resource.LoadBinaryFromFileSystem($"Assets/AAAGame/GenerateData/bytes/{file}.bytes"));

        }
    }
}


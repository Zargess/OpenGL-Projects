using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK_Test_project.Utility;

namespace OpenTK_Test_project.Models
{
    public class Block
    {
        private readonly float x, y, z;
        private const float sideRadius = 1.0f;
        private const float z_offset = 0.0f;
        private Vector4[] vertices;
        public bool Visible { get; private set; }

        public Block(float x, float y, float z, Material material = Material.Dirt)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            Visible = false;
            vertices = new Vector4[]
            {
                new Vector4(x-sideRadius, y-sideRadius, z+sideRadius + z_offset, 1.0f),
                new Vector4(x-sideRadius, y+sideRadius, z+sideRadius + z_offset, 1.0f),
                new Vector4(x+sideRadius, y+sideRadius, z+sideRadius + z_offset, 1.0f),
                new Vector4(x+sideRadius, y-sideRadius, z+sideRadius + z_offset, 1.0f),
                new Vector4(x-sideRadius, y-sideRadius, z-sideRadius + z_offset, 1.0f),
                new Vector4(x-sideRadius, y+sideRadius, z-sideRadius + z_offset, 1.0f),
                new Vector4(x+sideRadius, y+sideRadius, z-sideRadius + z_offset, 1.0f),
                new Vector4(x+sideRadius, y-sideRadius, z-sideRadius + z_offset, 1.0f)
            };

        }

        public void ToggleVisibility()
        {
            Visible = !Visible;
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        public List<BlockSide> GetBlockSides()
        {
            var res = new List<BlockSide>();

            res.Add(GetSide(1, 0, 3, 2));
            res.Add(GetSide(2, 3, 7, 6));
            res.Add(GetSide(3, 0, 4, 7));
            res.Add(GetSide(6, 5, 1, 2));
            res.Add(GetSide(4, 5, 6, 7));
            res.Add(GetSide(5, 4, 0, 1));
            
            return res;
        }

        private BlockSide GetSide(int a, int b, int c, int d)
        {
            var vertices = new[]
            {
                this.vertices[a],
                this.vertices[b],
                this.vertices[c],
                this.vertices[a],
                this.vertices[c],
                this.vertices[d]
            };
            var normal = Extensions.Cross(Vector4.Subtract(this.vertices[b], this.vertices[a]), Vector4.Subtract(this.vertices[c], this.vertices[a]));
            return new BlockSide
            {
                SideVertices = vertices,
                NormalVector = normal
            };
        }
    }
}

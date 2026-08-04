namespace Black_Hole
{
    using System.IO;
    using System.Numerics;

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            render_image();
            Application.Run(new Form1());

        }


        static void render_image()
        {
            int width = 16 * 60;
            int height = 9 * 60;
            for (int i = 0; i < 2000; i++)
            {
                using var stream = File.Create($"red_{i}.ppm");
                using var writer = new BinaryWriter(stream);
                string header = $"P6\n{width} {height}\n255\n";
                writer.Write(System.Text.Encoding.ASCII.GetBytes(header));
                Vector2 r = new Vector2(width, height);
                float t = i / 60.0f;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Vector4 o = new Vector4(0, 0, 0, 0);
                        Vector2 FC = new Vector2(x, y);
                        Vector2 p = (FC * 2 - r) / r.Y;
                        float l = Math.Abs(.7f - Dot(p, p));
                        Vector2 v = p * (1 - (l)) / .2f;

                        for (float j = 1.0f; j <= 8.0; j++)
                        {
                            o += Sin(v.XYYX()) + Vector4.One * MathF.Abs(v.X - v.Y) * .2f;
                            v += Cos(v.YX() * j + new Vector2(0.0f, j) + new Vector2(t)) / j + new Vector2(0.7f);

                        }

                        o = Tanh(Div(Exp(p.Y * new Vector4(1, -1, -2, 0)) * new Vector4(MathF.Exp(-4 * l)), o));
                        writer.Write((byte)Math.Clamp(o.X * 255f, 0f, 255f));
                        writer.Write((byte)Math.Clamp(o.Y * 255f, 0f, 255f));
                        writer.Write((byte)Math.Clamp(o.Z * 255f, 0f, 255f));
                    }
                }
                writer.Close();
                Console.WriteLine($"Generated red_{i}.ppm");
            }
        }

        static Vector2 YX(this Vector2 v)
        {
            return new Vector2(v.Y, v.X);
        }
        static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector4 XYYX(this Vector2 v)
        {
            return new Vector4(v.X, v.Y, v.Y, v.X);
        }

        public static Vector4 Tanh(this Vector4 v)
        {
            return new Vector4(
                MathF.Tanh(v.X),
                MathF.Tanh(v.Y),
                MathF.Tanh(v.Z),
                MathF.Tanh(v.W));
        }

        public static Vector4 Sin(this Vector4 v)
        {
            return new Vector4(
                MathF.Sin(v.X),
                MathF.Sin(v.Y),
                MathF.Sin(v.Z),
                MathF.Sin(v.W));
        }

        public static Vector4 Cos(this Vector4 v)
        {
            return new Vector4(
                MathF.Cos(v.X),
                MathF.Cos(v.Y),
                MathF.Cos(v.Z),
                MathF.Cos(v.W));
        }

        public static Vector2 Cos(this Vector2 v)
        {
            return new Vector2(
                MathF.Cos(v.X),
                MathF.Cos(v.Y));
        }

        public static Vector4 Exp(this Vector4 v)
        {
            return new Vector4(
                MathF.Exp(v.X),
                MathF.Exp(v.Y),
                MathF.Exp(v.Z),
                MathF.Exp(v.W));
        }

        public static Vector4 Div(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }
    }
}
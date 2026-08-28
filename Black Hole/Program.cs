namespace Black_Hole
{
    using System.IO;
    using System.Numerics;
    using System.Drawing;
    using System.Drawing.Imaging;

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
            Application.Run(new Form1());
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

        private static readonly Vector4 expMul = new Vector4(1, -1, -2, 0);
        private static readonly Vector2 offset = new Vector2(0.7f);
        private static readonly int width = 16 * 60;
        private static readonly int height = 9 * 60;
        private static Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        public static Bitmap render_image(float t)
        {
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* ptr = (byte*)data.Scan0;

                    for (int x = 0; x < width; x++)
                    {
                        (int, int, int) color;
                        switch (currentSlide)
                        {
                            case 0:
                                color = calculateCoolWaveRGB(x, y, t);
                                break;
                            case 1:
                                color = calculateWaveRGB(x, y, t);
                                break;
                            case 2:
                                color = calculateBlackHoleRGB(x, y, t);
                                break;
                            default:
                                color = (0, 0, 0);
                                break;
                        }
                        int red = color.Item1;
                        int blue = color.Item2;
                        int green = color.Item3;
                        ptr[(y * data.Stride) + x * 3] = (byte)blue;
                        ptr[(y * data.Stride) + x * 3 + 1] = (byte)green;
                        ptr[(y * data.Stride) + x * 3 + 2] = (byte)red;
                    };
                });
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        private static float Length(Vector4 v) => MathF.Sqrt(v.X* v.X + v.Y* v.Y + v.Z* v.Z + v.W* v.W);

        private static (int, int, int) calculateCoolWaveRGB(float x, float y, float t)
        {
            Vector4 o = Vector4.Zero;
            float z = 0.1f;
            float d = 0.1f;
            Vector2 FC = new Vector2(x, y);
            Vector2 r = new Vector2(width, height);
            for (float i = 0; i < 50; i++)
            {
                z += d;
                o += (new Vector4(0.9f) +
                Sin(new Vector4(i * 0.1f) - new Vector4(6, 1, 2, 0)))
                / (d * d * z);
                o += new Vector4(d * z) / new Vector4(4, 2, 1, 0);
                Vector3 dir = Vector3.Normalize(
                new Vector3((FC * 2f).X, (FC*2f).Y, 0f) - new Vector3(r.X, r.Y, r.X));
                Vector3 p = z * dir;
                d = 0f;
                while (++d < 9f)
                {
                    p += 0.4f *
                    new Vector3(
                    MathF.Sin(p.Y * d - z + t + i),
                    MathF.Sin(p.Z * d - z + t + i),
                    MathF.Sin(p.X * d - z + t + i))
                    / d
                    + new Vector3(0.5f);
                }
                d = Length(new Vector4(
                MathF.Abs(p.Y + p.Z * 0.5f),
                MathF.Sin(p.X - z) / 7f,
                MathF.Sin(p.Y - z) / 7f,
                MathF.Sin(p.Z - z) / 7f))
                / (4f + z * z / 100f);
            }
            o = Tanh(o / 2000f);
            int red = (int)Math.Clamp(o.X * 255f, 0f, 255f);
            int blue = (int)Math.Clamp(o.Y * 255F, 0f, 255f);
            int green = (int)Math.Clamp(o.Z * 255f, 0f, 255f);
            return ((int)red, (int)green, (int)blue);
        }

        private static (int, int, int) calculateWaveRGB(float x, float y, float t)
        {
            float r = MathF.Sqrt(x * x + y * y);
            float angle = MathF.Atan2(y, x);
            float wave = MathF.Sin(r * 0.1f - t) * 0.5f + 0.5f;
            float red = wave * 255f;
            float green = (MathF.Sin(angle + t) * 0.5f + 0.5f) * 255f;
            float blue = (MathF.Cos(angle - t) * 0.5f + 0.5f) * 255f;
            return ((int)red, (int)green, (int)blue);
        }

        private static (int, int, int) calculateBlackHoleRGB(float x, float y, float t)
        {
            Vector4 o = new Vector4(0, 0, 0, 0);
            Vector2 FC = new Vector2(x, y);
            Vector2 r = new Vector2(width, height);
            Vector2 p = (FC * 2 - r) / r.Y;
            float l = Math.Abs(.7f - Dot(p, p));
            Vector2 v = p * (1 - (l)) / .2f;
            for (float j = 1.0f; j <= 8.0; j++)
            {
                o += Sin(v.XYYX()) + Vector4.One * MathF.Abs(v.X - v.Y) * .2f;
                v += Cos(v.YX() * j + new Vector2(0.0f, j) + new Vector2(t)) / j + offset;
            }
            o = Tanh(Div(Exp(p.Y * expMul) * new Vector4(MathF.Exp(-4 * l)), o));
            int red = (int)Math.Clamp(o.X * 255f, 0f, 255f);
            int blue = (int)Math.Clamp(o.Y * 255F, 0f, 255f);
            int green = (int)Math.Clamp(o.Z * 255f, 0f, 255f);
            return (red, green, blue);
        }

        private static int currentSlide = 0;

        public static void ToggleSlide()
        {
            currentSlide = (currentSlide + 1) % 3;
        }
    }
}
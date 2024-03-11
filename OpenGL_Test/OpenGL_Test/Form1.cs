using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

// для работы с библиотекой OpenGL
using Tao.OpenGl;
// для работы с библиотекой FreeGLUT
using Tao.FreeGlut;
// для работы с элементом управления SimpleOpenGLControl
using Tao.Platform.Windows;


namespace OpenGL_Test
{

    //================================[Form]================================
    public partial class Form1 : Form
    {
        float[] red = new float[3] { 1, 0, 0 };
        float[] green = new float[3] { 0, 1, 0 };
        float[] blue = new float[3] { 0, 0, 1 };
        float[] yellow = new float[3] { 1, 1, 0 };
        float[] black = new float[3] { 0, 0, 0 };
        float[] gray = new float[3] { 0.5F, 0.5F, 0.5F};
        float[] orange = new float[3] { 0.75F, 0.4F, 0};

        int canvasWidth = 1400;
        int canvasHeight = 1000;

        public static Dot3D dot0 = new Dot3D(0, 0, 0);

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //создание класса Canvas и Viewport
            Canvas canvas = new Canvas(canvasWidth, canvasHeight);
            Viewport viewport = new Viewport(dot0, (float)canvasWidth / canvasHeight, 1, 1);

            Scene scene = new Scene();
            scene.CreateSphere(new Dot3D(0, -0.4F, 3), 0.6F, red, 500, (float)0.2);
            scene.CreateSphere(new Dot3D(-2.5F, 0, 6), (float)1.1, orange, 10, (float)0.4);
            scene.CreateSphere(new Dot3D(2.5F, 1, 7), (float)1.2, blue, 500, (float)0.3);
            scene.CreateSphere(new Dot3D(0, -5001, 0), 5000, gray, 1000, (float)0.5);

            scene.CreateLight(LightType.ambient, (float)0.2);
            scene.CreateLight(LightType.point, (float)0.6, new Dot3D(2, 1, 0));
            scene.CreateLight(LightType.directional, (float)0.2, new Vector3D(1, 4, 4));


            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            Gl.glBegin(Gl.GL_POINTS);
            for (int x = (-canvasWidth / 2); x < (canvasWidth / 2); x++)
            {
                for (int y = -canvasHeight / 2; y < canvasHeight / 2; y++)
                {
                    Ray3D D = viewport.CanvasToViewport(ref canvas, x, y);
                    float[] color = (float[])Ray3D.TraceRay(ref scene, ref D, 1, 1000000);
                    Gl.glColor3fv(color);

                    Gl.glVertex2f(x, y);
                }
            }
            Gl.glEnd();
            Gl.glFlush();
            AnT.Invalidate();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AnT_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация Glut
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);


            // очистка окна
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода в соответствии с размерами элемента anT
            Gl.glViewport(0, 0, canvasWidth, canvasHeight);
            // настройка проекции
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            // теперь необходимо корректно настроить 2D ортогональную проекцию
            Glu.gluOrtho2D(-canvasWidth / 2, canvasWidth / 2, -canvasHeight / 2, canvasHeight / 2);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }
    }

    public class Dot3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Dot3D(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Dot3D(Dot3D new_dot)
        {
            X = new_dot.X;
            Y = new_dot.Y;
            Z = new_dot.Z;
        }
        public Dot3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public static Dot3D operator *(Dot3D d, float a)
        => new Dot3D(d.X * a, d.Y * a, d.Z * a);
        public static Dot3D operator *(float a, Dot3D d)
        => new Dot3D(d.X * a, d.Y * a, d.Z * a);

        public static Dot3D operator +(Dot3D d, float a)
        => new Dot3D(d.X + a, d.Y + a, d.Z + a);
        public static Dot3D operator +(float a, Dot3D d)
        => new Dot3D(d.X + a, d.Y + a, d.Z + a);
        public static Dot3D operator +(Dot3D d1, Dot3D d2)
        => new Dot3D(d1.X + d2.X, d1.Y + d2.Y, d1.Z + d2.Z);
        public static Vector3D operator -(Dot3D d1, Dot3D d2)
        => new Vector3D(d2, d1);
    }

    public class Vector3D
    {
        public Dot3D Begin { get; set; }
        public Dot3D End { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3D(Dot3D begin, Dot3D end)
        {
            Begin = begin;
            End = end;
            X = end.X - begin.X;
            Y = end.Y - begin.Y;
            Z = end.Z - begin.Z;
        }
        public Vector3D(Dot3D end)
        {
            Begin = new Dot3D(0, 0, 0);
            End = end;
            X = end.X - Begin.X;
            Y = end.Y - Begin.Y;
            Z = end.Z - Begin.Z;
        }
        public Vector3D(float x = 0, float y = 0, float z = 0)
        {
            Begin = new Dot3D(0, 0, 0);
            End = new Dot3D(x, y, z);
            X = x;
            Y = y;
            Z = z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static Vector3D Cross(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }

        public static Vector3D operator *(Vector3D v1, float a)
        => new Vector3D(v1.X * a, v1.Y * a, v1.Z * a);
        public static Vector3D operator *(float a, Vector3D v1)
        => new Vector3D(v1.X * a, v1.Y * a, v1.Z * a);

        public static Vector3D operator +(Vector3D v1, float a)
        => new Vector3D(v1.X + a, v1.Y + a, v1.Z + a);
        public static Vector3D operator +(float a, Vector3D v1)
        => new Vector3D(v1.X + a, v1.Y + a, v1.Z + a);

        public static Vector3D operator +(Dot3D d, Vector3D v1)
        => new Vector3D(v1.X + d.X, v1.Y + d.Y, v1.Z + d.Z);
        public static Vector3D operator +(Vector3D v1, Dot3D d)
        => new Vector3D(v1.X + d.X, v1.Y + d.Y, v1.Z + d.Z);

        public static Vector3D operator -(Vector3D v1, float a)
        => new Vector3D(v1.X - a, v1.Y - a, v1.Z - a);

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        => new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

        public static float operator *(Vector3D v1, Vector3D v2)
        => (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z);
        public static Vector3D operator /(Vector3D v, float a)
        => new Vector3D(v.X / a, v.Y / a, v.Z / a);
    }

    public class Sphere
    {
        public Dot3D Center { get; set; }
        public float Radius { get; set; }
        public float[] Color { get; set; }
        public int Specular { get; set; }
        public float Reflective {  get; set; }

        public Sphere(Dot3D center, float radius, float[] color, int specular = 500, float reflective = 0)
        {
            Center = center;
            Radius = radius;
            Color = color;
            Specular = specular;
            Reflective = reflective;
        }

        public List<float> IntersectRaySphere(ref Ray3D ray)
        {
            Vector3D oc = new Vector3D(Center, ray.DotBegin);

            float k1 = ray.VectorD * ray.VectorD;
            float k2 = oc * ray.VectorD * 2;
            float k3 = oc * oc - Radius * Radius;

            float discriminant = (k2 * k2) - (4 * (k1 * k3));
            if (discriminant < 0) { return new List<float>{1000000, 1000000}; }
            else
            {
                float t1 = (-k2 + (float)Math.Sqrt(discriminant)) / (2 * k1);
                float t2 = (-k2 - (float)Math.Sqrt(discriminant)) / (2 * k1);
                return new List<float>{ t1, t2};
            }
        }
    }

    public class Scene
    {
        public List<Sphere> Spheres { get; set; }
        public List<Light> Lights { get; set; }

        public Scene()
        {
            Spheres = new List<Sphere>();
            Lights = new List<Light>();
        }

        public void CreateSphere
            (
            Dot3D center,
            float radius,
            float[] color,
            int specular,
            float reflective
            )
        {
            Spheres.Add(new Sphere(center, radius, color, specular, reflective));
        }
        public void CreateLight(LightType type, float intensity)
        {
            Lights.Add(new Light(type, intensity));
        }
        public void CreateLight(LightType type, float intensity, Dot3D position)
        {
            Lights.Add(new Light(type, intensity, position));
        }
        public void CreateLight(LightType type, float intensity, Vector3D direction)
        {
            Lights.Add(new Light(type, intensity, direction));
        }
    }

    public class Ray3D
    {
        public Dot3D DotBegin { get; set; }
        public Vector3D VectorD {  get; set; }

        public Ray3D(Dot3D dot_begin, Vector3D vector_d)
        {
            DotBegin = dot_begin;
            VectorD = vector_d.End - dot_begin;
            VectorD = VectorD / VectorD.Length();
        }

        public Ray3D(Vector3D vector_d)
        {
            DotBegin = Form1.dot0;
            VectorD = vector_d / vector_d.Length();
        }

        public static float ComputeLighting(ref Scene scene, ref Dot3D P, ref Vector3D N, Vector3D V, int specular)
        {
            float i = (float)0.0;
            foreach (Light light in scene.Lights)
            {
                if (light.Type == LightType.ambient)
                {
                    i += light.Intensity;
                }
                else
                {
                    Vector3D L;
                    float t_max;
                    if (light.Type == LightType.point)
                    {
                        L = light.Position - P;
                        t_max = 1;
                    }
                    else
                    {
                        L = light.Direction;
                        t_max = 1000000;
                    }

                    // Проверка на Тень
                    Ray3D shadowRay = new Ray3D(P, L);
                    Tuple<Sphere, float> shadowSphAndT = ClosestIntersection(ref scene, ref shadowRay, (float)0.001, t_max);
                    if (shadowSphAndT.Item1 != null) continue;

                    // Диффузность
                    float n_dot_l = N * L;
                    if (n_dot_l > 0) i += light.Intensity * n_dot_l / (N.Length() * L.Length());

                    // Зеркальность (Почти повторяя формулу Фонга, но по очереди)
                    if (specular != -1)
                    {
                        Vector3D R = ReflectRay(L, ref N).VectorD;
                        float r_dot_v = R * V;
                        if (r_dot_v > 0) i += light.Intensity * (float)Math.Pow(r_dot_v / (R.Length() * V.Length()), specular);
                    }
                }
            }
            return i;
        }

        public static Ray3D ReflectRay(Vector3D R, ref Vector3D N)
        {
            return new Ray3D(2 * N * (N * R) - R);
        }

        public static Tuple<Sphere, float> ClosestIntersection(ref Scene scene, ref Ray3D ray, float t_min, float t_max)
        {
            float closest_t = 100000;   // infinity
            Sphere closest_sphere = null;

            foreach (Sphere sphere in scene.Spheres)
            {
                List<float> intersects = sphere.IntersectRaySphere(ref ray);
                float t1 = intersects[0];
                float t2 = intersects[1];
                if ((t_min < t1) && (t1 < t_max) && (t1 < closest_t))
                {
                    closest_t = t1;
                    closest_sphere = sphere;
                }
                if ((t_min < t2) && (t2 < t_max) && (t2 < closest_t))
                {
                    closest_t = t2;
                    closest_sphere = sphere;
                }
            }
            return Tuple.Create(closest_sphere, closest_t);
        }

        public static Array TraceRay(ref Scene scene, ref Ray3D ray, float t_min, float t_max, int depth = 0)
        {
            Tuple<Sphere, float> closestSphereAndT = ClosestIntersection(ref scene, ref ray, t_min, t_max);
            Sphere closest_sphere = closestSphereAndT.Item1;
            float closest_t = closestSphereAndT.Item2;

            if (closest_sphere == null) return new float[3] { (float)0.1, (float)0.1, (float)0.1 };

            // Вычислим локальный цвет
            Dot3D P = ray.DotBegin + closest_t * ray.VectorD.End;   // вычисление пересечения
            Vector3D N = P - closest_sphere.Center;                 // вычисление нормали сферы в точке пересечения
            N = N / N.Length();
            float K_light = ComputeLighting(ref scene, ref P, ref N, (float)-1 * ray.VectorD, closest_sphere.Specular);
            float[] local_color = new float[3]
            {
                closest_sphere.Color[0] * K_light,
                closest_sphere.Color[1] * K_light,
                closest_sphere.Color[2] * K_light
            };

            // При достижении предела или неотражающего объекта - завершить
            float r = closest_sphere.Reflective;
            if (depth <= 0 || r <= 0) return local_color;

            // Вычислим лучи отражения:
            Ray3D R = ReflectRay((float)-1 * ray.VectorD, ref N);
            R = new Ray3D(P, R.VectorD);
            float[] reflected_color = (float[])TraceRay(ref scene, ref R, (float)0.001, 1000000, depth - 1);

            return new float[3]
            {
                local_color[0] * (1 - r) + reflected_color[0] * r,
                local_color[1] * (1 - r) + reflected_color[1] * r,
                local_color[2] * (1 - r) + reflected_color[2] * r
            };
        }
    }

    public enum LightType
    {
        ambient = 0,        // Окружающее "фоновое" освещение
        point = 1,          // Точечный источник света
        directional = 2,    // Направленный источник света (Солнце)
    }

    public class Light
    {
        public LightType Type { set; get; }
        public float Intensity { set; get; }
        public Dot3D Position { set; get; }
        public Vector3D Direction { set; get; }
        public Light(LightType type, float intensity)
        {
            Type = type;
            Intensity = intensity;
        }
        public Light(LightType type, float intensity, Dot3D position)
        {
            Type = type;
            Intensity = intensity;
            Position = position;
        }
        public Light(LightType type, float intensity, Vector3D direction)
        {
            Type = type;
            Intensity = intensity;
            Direction = direction;
        }
    }

    /// <summary>
    /// Мольберт, на котором мы раскрашиваем пискели.
    /// </summary>
    public class Canvas
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Canvas(int width = 1000, int height = 1000)
        {
            Width = width;
            Height = height;
        }
    }

    /// <summary>
    /// "Рамка", через которую мы смотрим на "внутренний Мир".
    /// </summary>
    public class Viewport
    {
        /// <summary>
        /// Позиция самого наблюдателя в Мире.
        /// </summary>
        public Dot3D Dot0 { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        /// <summary>
        /// Дистанция до "рамки" от наблюдателя.
        /// </summary>
        public int Distanse { get; set; }

        public Viewport(Dot3D dot0, float width = 1, float height = 1, int distanse = 1)
        {
            Dot0 = dot0;
            Width = width;
            Height = height;
            Distanse = distanse;
        }
        public Ray3D CanvasToViewport(ref Canvas canvas, int x, int y)
        {
            return new Ray3D(new Vector3D(x * Width / canvas.Width, y * Height / canvas.Height, Distanse));
        }
    }
}

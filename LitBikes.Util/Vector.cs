//using System;
//namespace LitBikes.Util
//{
//    public class Vector
//    {
//        public double x;
//        public double y;

//        //// Why not eh
//        //public Vector(int x, int y)
//        //{
//        //    new Vector((double)x, (double)y);
//        //}

//        public Vector(double x, double y)
//        {
//            this.x = x;
//            this.y = y;
//        }

//        public void add(Vector that)
//        {
//            x += that.x;
//            y += that.y;
//        }

//        public static Vector Zero()
//        {
//            return new Vector(0, 0);
//        }

//        public static Vector Random(int maxX, int maxY)
//        {
//            return new Vector(new Random().Next(maxX), new Random().Next(maxX));
//        }
                
//        public override String ToString()
//        {
//            return "(" + x + ", " + y + ")";
//        }
//    }
//}

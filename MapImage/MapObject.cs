using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Runtime.Serialization;

using System.Diagnostics;
using System.Device;
using System.Device.Location;

namespace MapImage
{

    /// <summary>
    /// 頂点情報の管理
    /// </summary>
    [DataContract]
    public class Vertex3
    {
        [DataMemberAttribute]
        private float[] k;
        private int count;

        public float x { get { return k[0]; } }
        public float y { get { return k[1]; } }
        public float z { get { return k[2]; } }

        //  ポリゴンの構成情報
        public Vertex3(float x, float y, float z)
        {
            k = new float[3];
            k[0] = x;
            k[1] = y;
            k[2] = z;

            count = 1;
        }

        public void Increment()
        {
            count++;
        }
        public int Count()
        {
            return count;
        }
    }

    [DataContract]
    public class Polygon3
    {
        [DataMemberAttribute]
        private int[] i;

        public int i1 { get { return i[0]; } }
        public int i2 { get { return i[1]; } }
        public int i3 { get { return i[2]; } }

        //  ポリゴンの構成情報
        public Polygon3(int i1, int i2, int i3)
        {
            i = new int[3];
            i[0] = i1;
            i[1] = i2;
            i[2] = i3;
        }
    }

    /// <summary>
    /// 建物情報の保持
    /// </summary>
    [DataContract]
    public class BuildInfo
    {
        //  建物の頂点情報
        [DataMemberAttribute]
        private List<Vertex3> v;
        //  ポリゴンを構成する頂点のリスト
        [DataMemberAttribute]
        private List<Polygon3> p;
        [DataMemberAttribute]
        public float startPosX;
        [DataMemberAttribute]
        public float startPosY;
        [DataMemberAttribute]
        public float endPosX;
        [DataMemberAttribute]
        public float endPosY;


        public List<Vertex3> getVertex3 { get { return v; } }
        public int AddVertex3(Vertex3 item)
        {
            for (int i = 0; i < v.Count(); i++)
            {
                if (v[i].x == item.x && v[i].y == item.y && v[i].z == item.z)
                {
                    v[i].Increment();
                    return i;
                }
            }

            v.Add(item);

            return v.Count() - 1;
        }
        public int CountVertex3()
        {
            return v.Count();
        }
        public List<Polygon3> getPolygon3 { get { return p; } }
        public void AddPolygon3(Polygon3 item)
        {
            p.Add(item);
        }
        public int CountPolygon3()
        {
            return p.Count();
        }
        public BuildInfo()
        {
            this.v = new List<Vertex3>();
            this.p = new List<Polygon3>();
        }

        public void Release()
        {
            this.v.Clear();
            this.p.Clear();
        }
    }


    /// <summary>
    /// 地図情報のJsonファイル情報を保持
    /// </summary>
    [DataContract]
    public class MapInfoJson
    {

        private static string filePath = /*Application.dataPath*/ "c:\temp" + "/mapinfo.json";//セーブデータのファイルパス
        public static string FilePath
        {//ファイルパスのプロパティ
            get { return filePath; }
        }

        [DataMemberAttribute]
        public List<BuildInfo> map;

        public void Add(BuildInfo item)
        {
            map.Add(item);
        }

        public List<BuildInfo> List { get { return map; } }

        public int Count() { return map.Count(); }

        public MapInfoJson()
        {
            this.map = new List<BuildInfo>();
        }
    }

    public class MapObject
    {
        public Point2d BoxMin;
        public Point2d BoxMax;

        private Mat image;
        private Point2f relPt;
        private double width;
        private double height;
        public List<Point2f> keypoints;
        public List<Point2f> dist_points;
        public List<Side> sides;
        public BuildInfo build;

        public MapObject()
        {
            image = null;
            relPt.X = 0;
            relPt.Y = 0;

            width = 0;
            height = 0;

            keypoints = new List<Point2f>();
            dist_points = new List<Point2f>();
            sides = new List<Side>();
            build = new BuildInfo();
        }

        public void CalcSize()
        {
            double minX = 10000, maxX = 0;
            double minY = 10000, maxY = 0;
            double TokyoX = 139.767052; 
            double TokyoY = 35.681167;

            var x = new GeoCoordinate(0, BoxMin.X).GetDistanceTo(new GeoCoordinate(0, TokyoX));
            var y = new GeoCoordinate(BoxMin.Y, 0).GetDistanceTo(new GeoCoordinate(TokyoY, 0));

            if(TokyoX < BoxMin.X)
            {
                relPt.X = (float)x;
            }
            else
            {
                relPt.X = (float)-x;
            }
            if (TokyoY < BoxMin.Y)
            {
                relPt.Y = (float)y;
            }
            else
            {
                relPt.Y = (float)-y;
            }
            build.startPosX = relPt.X;
            build.startPosY = relPt.Y;

            x = new GeoCoordinate(0, BoxMax.X).GetDistanceTo(new GeoCoordinate(0, TokyoX));
            y = new GeoCoordinate(BoxMax.Y, 0).GetDistanceTo(new GeoCoordinate(TokyoY, 0));

            if (TokyoX < BoxMax.X)
            {
                build.endPosX = (float)x;
            }
            else
            {
                build.endPosX = (float)-x;
            }
            if (TokyoY < BoxMax.Y)
            {
                build.endPosY = (float)y;
            }
            else
            {
                build.endPosY = (float)-y;
            }


            for ( int i = 0; i< dist_points.Count(); i++)
            {
                if( minX > dist_points[i].X)
                {
                    minX = dist_points[i].X;
                }
                if (maxX < dist_points[i].X)
                {
                    maxX = dist_points[i].X;
                }
                if (minY > dist_points[i].Y)
                {
                    minY = dist_points[i].Y;
                }
                if (maxY < dist_points[i].Y)
                {
                    maxY = dist_points[i].Y;
                }
            }

            if(minX < maxX && minY < maxY)
            {
                //relPt.X = (float)minX;
                //relPt.Y = (float)minY;
                width = maxX;
                height = maxY;
            }
        }

        public void SetRect( double Xmin, double Ymin, double Xmax, double Ymax )
        {
            BoxMin.X = Xmin;
            BoxMin.Y = Ymin;
            BoxMax.X = Xmax;
            BoxMax.Y = Ymax;
        }

        public void AddKeyPoint( double x, double y)
        {
            keypoints.Add(new Point2f( (float)x, (float)y ) );
        }



        public void Long2Dist()
        {
            for (int i = 0; i < keypoints.Count(); i++ ){
                double x = new GeoCoordinate(0, BoxMin.X).GetDistanceTo(new GeoCoordinate(0, keypoints[i].X));
                double y = new GeoCoordinate(BoxMin.Y, 0).GetDistanceTo(new GeoCoordinate( keypoints[i].Y, 0));

                dist_points.Add(new Point2f((float)x, (float)y));
            }
        }

        public MapObject(Mat obj, float startX, float startY)
        {
            image = obj;
            relPt.X = startX;
            relPt.Y = startY;

            width = image.Width;
            height = image.Height;


            build = new BuildInfo();

        }



        public KeyPoint[] toKeyPoints()
        {
            List<KeyPoint> listKeyPoint = new List<KeyPoint>();

            for (int i = 0; i < this.keypoints.Count(); i++)
            {
                listKeyPoint.Add(new KeyPoint(this.keypoints[i].X, this.keypoints[i].Y, 7));
            }
            return listKeyPoint.ToArray<KeyPoint>();
        }

        public int Count()
        {
            return this.sides.Count();
        }

        private void Swap(int i, int j)
        {
            Side Temp;
            Temp = this.sides[i];
            this.sides[i] = this.sides[j];
            this.sides[j] = Temp;

        }

        public void DeleteInsideLine()
        {
            for (int i = this.sides.Count() - 1; i >= 0; i--)
            {
                if (this.sides[i].Count() >= 2)
                {
                    this.sides.RemoveAt(i);
                }
            }
        }

        private bool OnLine(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            bool bRet = false;

            if (x1 < x2)
            {
                if (x1 <= x3 && x3 <= x2)
                {
                    bRet = true;
                }
            }
            else
            {
                if (x2 <= x3 && x3 <= x1)
                {
                    bRet = true;
                }
            }

            if (bRet != false)
            {
                bRet = false;

                if (y1 < y2)
                {
                    if (y1 <= y3 && y3 <= y2)
                    {
                        bRet = true;
                    }
                }
                else
                {
                    if (y2 <= y3 && y3 <= y1)
                    {
                        bRet = true;
                    }
                }
            }

            return bRet;
        }


        public void DeleteExtraPoint()
        {

            this.keypoints.Clear();

            for (int i = 0; i < this.sides.Count(); i++)
            {
                if (this.keypoints.Count() == 0)
                {
                    this.keypoints.Add(this.sides[i].Item0);
                }
                this.keypoints.Add(this.sides[i].Item1);
            }

            int j;
            do
            {
                if (this.keypoints.Count() <= 4)
                {
                    break;
                }

                for (j = 0; j < this.keypoints.Count(); j++)
                {
                    Point2f prevPt;
                    Point2f nextPt;

                    if (j - 1 < 0)
                    {
                        prevPt = this.keypoints[this.keypoints.Count() - 1];
                    }
                    else
                    {
                        prevPt = this.keypoints[j - 1];
                    }

                    if (j + 1 >= this.keypoints.Count())
                    {
                        nextPt = this.keypoints[0];
                    }
                    else
                    {
                        nextPt = this.keypoints[j + 1];
                    }

                    double x1, x2, y1, y2, a, b, c, d;

                    x1 = prevPt.X;
                    x2 = nextPt.X;
                    y1 = prevPt.Y;
                    y2 = nextPt.Y;


                    a = (y2 - y1) / (x2 - x1);
                    b = -1;
                    c = y1 - a * x1;

                    double temp1 = Math.Abs(a * this.keypoints[j].X + b * this.keypoints[j].Y + c);
                    double temp2 = Math.Sqrt(a * a + b * b);
                    d = temp1 / temp2;

                    if (d < 1)
                    {
                        double c2;

                        c2 = this.keypoints[j].Y - (-1 * a) * this.keypoints[j].X;

                        double crossX;
                        double crossY;

                        crossX = (c2 - c) / (2 * a);
                        crossY = (c + c2) / 2;

                        if (OnLine(x1, y1, x2, y2, crossX, crossY) != false)
                        {
                            this.keypoints.RemoveAt(j);
                            j--;
                        }
                    }
                }
            } while (j < this.keypoints.Count());

            this.sides.Clear();

            for (int k = 0; k < this.keypoints.Count(); k++)
            {
                if (k < this.keypoints.Count() - 1)
                {
                    AddSide((int)this.keypoints[k].X, (int)this.keypoints[k].Y, (int)this.keypoints[k + 1].X, (int)this.keypoints[k + 1].Y);
                }
                else
                {
                    AddSide((int)this.keypoints[k].X, (int)this.keypoints[k].Y, (int)this.keypoints[0].X, (int)this.keypoints[0].Y);
                }
            }
        }

        public void SortItems()
        {
            if (this.sides.Count() > 0)
            {
                Point2f lastItem;
                int i = 0;
                //  一つ目は時計周りか確認
                if (this.sides[i].Item0.X > this.sides[i].Item1.X
                || (this.sides[i].Item0.X == this.sides[i].Item1.X && this.sides[i].Item0.Y > this.sides[i].Item1.Y)
                )
                {
                    Point2f temp = this.sides[i].Item0;
                    this.sides[i].Item0 = this.sides[i].Item1;
                    this.sides[i].Item1 = temp;
                }

                if (this.sides.Count() > 1)
                {
                    for (i = 1; i < this.sides.Count() - 1; i++)
                    {
                        lastItem = this.sides[i - 1].Item1;

                        for (int j = i; j < this.sides.Count(); j++)
                        {
                            if (lastItem.X == this.sides[j].Item0.X && lastItem.Y == this.sides[j].Item0.Y)
                            {
                                Swap(i, j);
                                break;
                            }
                            else if (lastItem.X == this.sides[j].Item1.X && lastItem.Y == this.sides[j].Item1.Y)
                            {
                                Point2f temp = this.sides[j].Item0;
                                this.sides[j].Item0 = this.sides[j].Item1;
                                this.sides[j].Item1 = temp;

                                Swap(i, j);

                                break;
                            }
                        }
                    }

                    //　最後のアイテムのデータチェック
                    int lastidx = this.sides.Count() - 1;
                    lastItem = this.sides[lastidx - 1].Item1;
                    if (lastItem.X == this.sides[lastidx].Item1.X && lastItem.Y == this.sides[lastidx].Item1.Y)
                    {
                        Point2f temp = this.sides[lastidx].Item0;
                        this.sides[lastidx].Item0 = this.sides[lastidx].Item1;
                        this.sides[lastidx].Item1 = temp;
                    }
                }
            }
        }

        public int AddSide(float x1, float y1, float x2, float y2)
        {
            int nRet = 0;
            bool isAlreadyExist = false;

            for (int i = 0; i < this.sides.Count(); i++)
            {
                if (this.sides[i].Equals(new Side(x1, y1, x2, y2)) != false)
                {
                    this.sides[i].IncrementCount();
                    nRet = i;
                    isAlreadyExist = true;

                    break;
                }
            }

            if (isAlreadyExist == false)
            {
                this.sides.Add(new Side(x1, y1, x2, y2));

                nRet = this.sides.Count() - 1;
            }

            return nRet;
        }

        public Point2f RelPt
        {
            get { return relPt; }
            //set { _objMat = value; }
        }

        public Mat ImgData
        {
            get { return image; }
            //set { _objMat = value; }
        }

        public double Width
        {
            get { return width; }
        }

        public double Height
        {
            get { return height; }
        }
    }

    //  お家用
    public class House : MapObject
    {

        public House(Mat obj, float startX, float startY) : base(obj, startX, startY)
        {
            keypoints = new List<Point2f>();
            sides = new List<Side>();

            GetKeyPoint();
            GetSides();
            //  一度オブジェクトの内側の線を削除する
            DeleteInsideLine();
            //  頂点の順番をそろえつつ、頂点リストを作成
            SortItems();
            //  省けそうな頂点を削除する
            DeleteExtraPoint();
            //  余分な頂点を削除した状態で再度、辺情報を作成する
            GetSides();
            AddHeight();
        }
        public House() : base()
        {
        }

        private int GetKeyPoint()
        {

            //------AGAST-------------
            OpenCvSharp.AgastFeatureDetector agast = OpenCvSharp.AgastFeatureDetector.Create();
            OpenCvSharp.KeyPoint[] points;

            points = agast.Detect(this.ImgData);

            for (int i = 0; i < points.Count(); i++)
            {
                keypoints.Add(new Point2f(points[i].Pt.X, points[i].Pt.Y));
            }

            agast.Dispose();

            return keypoints.Count();
        }

        public void MakePolygon()
        {
            GetSides();

            AddHeight();
        }
        private void AddHeight()
        {
            Random cRandom = new System.Random();
            float dummyheight = cRandom.Next(10, 150); ;

            int nPoly = this.build.CountPolygon3();
            for (int l = 0; l < nPoly; l++)
            {
                int i1 = this.build.getPolygon3[l].i1;
                int i2 = this.build.getPolygon3[l].i2;
                int i3 = this.build.getPolygon3[l].i3;

                int i4, i5, i6;

                i4 = build.AddVertex3(new Vertex3(this.build.getVertex3[i1].x, dummyheight, this.build.getVertex3[i1].z));
                i5 = build.AddVertex3(new Vertex3(this.build.getVertex3[i2].x, dummyheight, this.build.getVertex3[i2].z));
                i6 = build.AddVertex3(new Vertex3(this.build.getVertex3[i3].x, dummyheight, this.build.getVertex3[i3].z));
                build.AddPolygon3(new Polygon3(i6, i5, i4));
            }

            for (int i = 0; i < this.Count(); i++)
            {
                if (this.sides[i].Count() == 1)
                {
                    int i1, i2, i3;
                    Point2f Item0;
                    Point2f Item1;

                    Item0 = this.sides[i].Item0;
                    Item1 = this.sides[i].Item1;

                    i1 = build.AddVertex3(new Vertex3(Item0.X + RelPt.X, 0, Item0.Y + RelPt.Y));
                    i2 = build.AddVertex3(new Vertex3(Item0.X + RelPt.X, dummyheight, Item0.Y + RelPt.Y));
                    i3 = build.AddVertex3(new Vertex3(Item1.X + RelPt.X, 0, Item1.Y + RelPt.Y));
                    build.AddPolygon3(new Polygon3(i1, i2, i3));

                    i1 = build.AddVertex3(new Vertex3(Item0.X + RelPt.X, dummyheight, Item0.Y + RelPt.Y));
                    i2 = build.AddVertex3(new Vertex3(Item1.X + RelPt.X, dummyheight, Item1.Y + RelPt.Y));
                    i3 = build.AddVertex3(new Vertex3(Item1.X + RelPt.X, 0, Item1.Y + RelPt.Y));
                    build.AddPolygon3(new Polygon3(i1, i2, i3));
                }
            }


        }

        private int GetSides()
        {
            build.Release();

            Subdiv2D subdiv = new Subdiv2D();

            subdiv.InitDelaunay(new Rect(0, 0, (int)(Width) + 100, (int)(Height) + 100));
            subdiv.Insert(this.dist_points);

            // ドロネー三角形のリストを取得
            Vec6f[] triangles;
            triangles = subdiv.GetTriangleList();

            //  辺を洗い出す
            this.sides.Clear();

            for (int l = 0; l < triangles.Count(); l++)
            {

                //(10, 20)-(100, 200)に、幅1の黒い線を引く
                Vec6f ptData = triangles[l];

                if ((0 <= ptData[0] && ptData[0] < (float)Width + 1)
                    && (0 <= ptData[1] && ptData[1] < (float)Height + 1)
                    && (0 <= ptData[2] && ptData[2] < (float)Width + 1)
                    && (0 <= ptData[3] && ptData[3] < (float)Height + 1)
                    && (0 <= ptData[4] && ptData[4] < (float)Width + 1)
                    && (0 <= ptData[5] && ptData[5] < (float)Height + 1)
                )
                {
                    if ( IsInArea(ptData) != false )
                    {
                        int i1, i2, i3;

                        AddSide(ptData[0], ptData[1], ptData[2], ptData[3]);
                        AddSide(ptData[2], ptData[3], ptData[4], ptData[5]);
                        AddSide(ptData[4], ptData[5], ptData[0], ptData[1]);

                        //  底辺として記憶する
                        i1 = build.AddVertex3(new Vertex3(ptData[0] + RelPt.X, 0, ptData[1] + RelPt.Y));
                        i2 = build.AddVertex3(new Vertex3(ptData[2] + RelPt.X, 0, ptData[3] + RelPt.Y));
                        i3 = build.AddVertex3(new Vertex3(ptData[4] + RelPt.X, 0, ptData[5] + RelPt.Y));
                        build.AddPolygon3(new Polygon3(i1, i2, i3));
                    }
                }
            }

            subdiv.Dispose();


            return this.sides.Count();

        }

        private bool IsInSidePoint(float x, float y)
        {
            bool bInside = false;

            for (int i = 0; i < this.keypoints.Count(); i++)
            {
                if (this.keypoints[i].X == (int)x && this.keypoints[i].Y == (int)y)
                {
                    return true;
                }
            }

            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }

            Vec3b px = ImgData.Get<Vec3b>((int)y, (int)x);

            if (px != new Vec3b(0xFF, 0xFF, 0xFF))
            {
                bInside = true;
            }

            return bInside;
        }

        private Boolean IsInArea(float x, float y)
        {
            double angle = 0;

            for (int i = 0; i < this.dist_points.Count(); i++)
            {
                Point2f pt1;
                Point2f pt2;

                pt1 = this.dist_points[i];
                if (i + 1 >= this.dist_points.Count())
                {
                    pt2 = this.dist_points[0];
                }
                else
                {
                    pt2 = this.dist_points[i + 1];
                }
                Vec2d v1;
                Vec2d v2;

                v1.Item0 = pt1.X - x;
                v1.Item1 = pt1.Y - y;
                v2.Item0 = pt2.X - x;
                v2.Item1 = pt2.Y - y;

                double innerPro;
                double crossPro;


                innerPro = v1.Item0 * v2.Item0 + v1.Item1 * v2.Item1;
                crossPro = v1.Item0 * v2.Item1 - v1.Item1 * v2.Item0;

                double temp_angle;

                temp_angle = (Math.Atan2(crossPro, innerPro) * 180) / Math.PI;

                angle += temp_angle;

            }

            if (359 < Math.Abs(angle) && Math.Abs(angle) < 361)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsInArea(Vec6f ptData)
        {
            //なんか大変そうなので重心にあるかだけみる
            float x = (float)((ptData[0] + ptData[2] + ptData[4]) / 3);
            float y = (float)((ptData[1] + ptData[3] + ptData[5]) / 3);


            return IsInArea(x, y);

        }
    }

    public class Road : MapObject
    {
        public Road(Mat obj, float startX, float startY) : base(obj, startX, startY)
        {
        }
        public Road() : base()
        {
        }

        public void MakeRoadLine()
        {
            for( int i = 0; i < this.dist_points.Count(); i++)
            {
                build.AddVertex3(new Vertex3(dist_points[i].X + RelPt.X, 3, dist_points[i].Y + RelPt.Y));
            }
        }
    }




    public class Side
    {
        public Point2f Item0;
        public Point2f Item1;
        public int count;

        public Side(float x1, float y1, float x2, float y2)
        {
            count = 1;
            Item0.X = x1;
            Item0.Y = y1;
            Item1.X = x2;
            Item1.Y = y2;
        }

        public int Count()
        {
            return count;
        }
        public int IncrementCount()
        {
            count++;

            return count;
        }

        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here
            //throw new NotImplementedException();
            Side target = (Side)obj;

            if (Item0.Equals(target.Item0) != false && Item1.Equals(target.Item1) != false)
            {
                return true;
            }
            else if (Item0.Equals(target.Item1) != false && Item1.Equals(target.Item0) != false)
            {
                return true;
            }
            return false;
            //return base.Equals(obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            //throw new NotImplementedException();

            return base.GetHashCode();
        }
    }

}

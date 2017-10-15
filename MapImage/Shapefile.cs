using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MapImage
{
    namespace shp
    {
        enum shptype {
            NULLSHAPE = 0,
            POINT,
            POLYLINE = 3,
            POLYGON = 5,
        };

        public class common
        {
            static public int ReadInt(byte[] pData, int i, Boolean bBig)
            {
                byte[] byteTemp = new byte[4];

                //	ビックエンディアン
                if (bBig == true)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        byteTemp[j] = pData[i + (4 - 1) - j];
                    }
                }
                //	リトルエンディアン
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        byteTemp[j] = pData[i + j];
                    }
                }

                return BitConverter.ToInt32(byteTemp, 0);
            }

            static public double ReadDouble(byte[] pData, int i, Boolean bBig)
            {
                byte[] byteTemp = new byte[8];

                //	ビックエンディアン
                if (bBig == true)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        byteTemp[j] = pData[i + (8 - 1) - j];
                    }
                }
                //	リトルエンディアン
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        byteTemp[j] = pData[i + j];
                    }
                }

                return BitConverter.ToDouble(byteTemp, 0);
            }
        }

        public class Contents<Type>
        {
            private int _nType;
            private List<Type> _listContents;

            public int nType
            {
                get { return _nType; }
                set { _nType = value; }
            }

            public List<Type> listContents
            {
                get { return _listContents; }
                set { _listContents = value; }
            }

            public Contents(){}
        }

        public class Shpfile
        {
            private string _path;
            private Header _header;
            private Contents<Polygon> _contents;

            public Header Header{
                get{
                    return _header;
                }
            }

            public Contents<Polygon> Contents
            {
                get { return _contents;  }
            }

            public Shpfile(string path)
            { 
               _path = path;

                System.IO.FileStream fs = new System.IO.FileStream( _path, System.IO.FileMode.Open, System.IO.FileAccess.Read );

                byte[] header = new byte[100];
                byte[] record = new byte[8];

                fs.Read(header, 0, 100);

                _header = shp.Header.Analyze(header, 100);

                _contents = new Contents<shp.Polygon>();

                _contents.nType = _header.ShapeType;
                _contents.listContents = new List<Polygon>();

                int nCurSize = 100;

                while (nCurSize <= fs.Length)
                {
                    fs.Read(record, 0, 8);

                    int nNo;
                    int nLength;

                    nNo = shp.common.ReadInt(record, 0, true);
                    nLength = shp.common.ReadInt(record, 4, true) * 2;

                    nCurSize += 8;

                    //コンテンツサイズ分読み込む
                    byte[] data = new byte[nLength];

                    fs.Read(data, 0, nLength);

                    if ( _header.ShapeType == (int)shp.shptype.POLYGON)
                    {
                        _contents.listContents.Add(shp.Polygon.Analyze(data, nLength));
                    }

                    nCurSize += nLength;
                }

                fs.Close();
            }

            public string path
            {
                get { return _path; }
                set { _path = value; }
            }
        }

        public class Header
        {
            private int _nFileCode;
            private int _nFileLength;
            private int _nVer;
            private int _nShapeType;
            private double _dBBXmin;
            private double _dBBYmin;
            private double _dBBXmax;
            private double _dBBYmax;
            private double _dBBZmin;
            private double _dBBZmax;
            private double _dBBMmin;
            private double _dBBMmax;

            public Header(
                int nFileCode, int nFileLength, int nVer, int nShapeType,
                double dBBXmin, double dBBYmin, double dBBXmax, double dBBYmax,
                double dBBZmin, double dBBZmax, double dBBMmin, double dBBMmax
                )
            {
                _nFileCode = nFileCode;
                _nFileLength = nFileLength;
                _nVer = nVer;
                _nShapeType = nShapeType;
                _dBBXmin = dBBXmin;
                _dBBYmin = dBBYmin;
                _dBBXmax = dBBXmax;
                _dBBYmax = dBBYmax;
                _dBBZmin = dBBZmin;
                _dBBZmax = dBBZmax;
                _dBBMmin = dBBMmin;
                _dBBMmax = dBBMmax;
            }

            public int ShapeType { get { return _nShapeType; } }


            static public Header Analyze(byte[] pData, uint nSize)
            {
                int i = 0;

                int _nFileCode;
                int _nFileLength;
                int _nVer;
                int _nShapeType;
                double _dBBXmin;
                double _dBBYmin;
                double _dBBXmax;
                double _dBBYmax;
                double _dBBZmin;
                double _dBBZmax;
                double _dBBMmin;
                double _dBBMmax;

                _nFileCode = common.ReadInt( pData, i, true);    i += 4;
                i += sizeof(int) * 5;
                _nFileLength = common.ReadInt(pData, i, true); i += 4;
                _nVer = common.ReadInt(pData, i, false); i += 4;
                _nShapeType = common.ReadInt(pData, i, false); i += 4;
                _dBBXmin = common.ReadDouble(pData, i, false); i += 8;
                _dBBYmin = common.ReadDouble(pData, i, false); i += 8;
                _dBBXmax = common.ReadDouble(pData, i, false); i += 8;
                _dBBYmax = common.ReadDouble(pData, i, false); i += 8;
                _dBBZmin = common.ReadDouble(pData, i, false); i += 8;
                _dBBZmax = common.ReadDouble(pData, i, false); i += 8;
                _dBBMmin = common.ReadDouble(pData, i, false); i += 8;
                _dBBMmax = common.ReadDouble(pData, i, false); i += 8;

                Header pHeader = new Header(
                    _nFileCode, _nFileLength, _nVer, _nShapeType,
                    _dBBXmin, _dBBYmin, _dBBXmax, _dBBYmax,
                    _dBBZmin, _dBBZmax, _dBBMmin, _dBBMmax
                );

                return pHeader;
            }
        };

        public class Point
        {
            private double _x;
            private double _y;

            public Point() { }
            public Point(double d1, double d2)
            {
                _x = d1;
                _y = d2;
            }

            public double x
            {
                get{ return _x;}
                set{ _x = value;}
            }
            public double y
            {
                get { return _y; }
                set { _y = value; }
            }

        };

        public class Polygon
        {
    		Polygon() { }

            private int _type;
            private double[] _Box;
            private int _nNumParts;
            private int _nNumPoints;
            private List<int> _listParts;
            private List<Point> _listPoints;

            public int nType
            {
                get { return _type; }
                set { _type = value; }
            }
            public double[] Box
            {
                get { return _Box; }
                set { _Box = value; }
            }

            public int nNumParts
            {
                get { return _nNumParts; }
                set { _nNumParts = value; }
            }

            public int nNumPoints
            {
                get { return _nNumPoints; }
                set { _nNumPoints = value; }
            }

            public List<int> listParts
            {
                get { return _listParts; }
                set { _listParts = value; }
            }

            public List<Point> listPoints
            {
                get { return _listPoints; }
                set { _listPoints = value; }
            }

            static public Polygon Analyze(byte[] pData, int nSize)
            {
                int i = 0;

                Polygon pPolygon = new Polygon();

                pPolygon.nType = common.ReadInt(pData, i, false); i += 4;
                pPolygon.Box = new double[4];
                pPolygon.Box[0] = common.ReadDouble(pData, i, false); i += 8;
                pPolygon.Box[1] = common.ReadDouble(pData, i, false); i += 8;
                pPolygon.Box[2] = common.ReadDouble(pData, i, false); i += 8;
                pPolygon.Box[3] = common.ReadDouble(pData, i, false); i += 8;
                pPolygon.nNumParts = common.ReadInt(pData, i, false); i += 4;
                pPolygon.nNumPoints = common.ReadInt(pData, i, false); i += 4;

                pPolygon.listParts = new List<int>();
                int j;
                for (j = 0; j < pPolygon.nNumParts; j++)
                {
                    int nPart = common.ReadInt(pData, i, false); i += 4;
                    pPolygon.listParts.Add(nPart);
                }

                pPolygon.listPoints = new List<Point>();
                for (j = 0; j < pPolygon.nNumPoints; j++)
                {
                    double x;
                    double y;

                    x = common.ReadDouble(pData, i, false); i += 8;
                    y = common.ReadDouble(pData, i, false); i += 8;

                    Point pt = new Point(x, y);
                    pPolygon.listPoints.Add(pt);
                }

                return pPolygon;
            }
        };
    }
}

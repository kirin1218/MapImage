using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using static System.Console;
using OpenCvSharp;
using System.Diagnostics;
using System.Device;
using System.Device.Location;

namespace MapImage
{
    public partial class Form1 : Form
    {
        Mat loadImage;

        Mat viewImage;
        List<House> listHouse;
        List<Road> listRoad;
        Rect subviewRect;
        int nListViewMode;

        private Rectangle drawRectangle;
        public Form1()
        {
            InitializeComponent();
           //ListViewコントロールの設定
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;

            //カラムの設定（3列）
            listView1.Columns.Add("No");
            listView1.Columns.Add("個数");
            listView1.Columns.Add("MinX");
            listView1.Columns.Add("MinY");
            listView1.Columns.Add("MaxX");
            listView1.Columns.Add("MaxY");

            listHouse = new List<House>();
            listRoad = new List<Road>();

            //ListViewコントロールの設定
            KeyPointslistView.View = View.Details;
            //KeyPointslistView.FullRowSelect = true;
            KeyPointslistView.GridLines = true;

            //カラムの設定（3列）
            KeyPointslistView.Columns.Add("X座標");
            KeyPointslistView.Columns.Add("Y座標");
        }

        private void SetImgColorCount()
        {
            Dictionary<Vec3b, int> dicColors;
            dicColors = new Dictionary<Vec3b, int>();

            for (int y = 0; y < loadImage.Height; y++)
            {
                for (int x = 0; x < loadImage.Width; x++)
                {
                    Vec3b px = loadImage.Get<Vec3b>(y, x);

                    if (px != new Vec3b(0xff, 0xff, 0xff))
                    {
                        if (dicColors.ContainsKey(px))
                        {
                            dicColors[px] = dicColors[px] + 1;
                        }
                        else
                        {
                            dicColors.Add(px, 1);
                        }
                    }
                }
            }

            foreach (var v in dicColors)
            {
                string[] row = new string[2];

                row[0] = string.Format("0x{0:X2}{1:X2}{2:X2}", v.Key[0], v.Key[1], v.Key[2]);
                row[1] = string.Format("{0}", v.Value);

                listView1.Items.Add(new ListViewItem(row));
            }
        }

        private void filterImage(List<Vec3b> v)
        {
            Mat filterImg;

            filterImg = loadImage.Clone();

            for (int y = 0; y < filterImg.Height; y++)
            {
                for (int x = 0; x < filterImg.Width; x++)
                {
                    Vec3b px = filterImg.Get<Vec3b>(y, x);

                    if (px != new Vec3b(0xff, 0xff, 0xff))
                    {
                        bool bMatch = false;

                        foreach (Vec3b n in v)
                        {
                            if (px == n)
                            {
                                bMatch = true;
                                break;
                            }
                        }

                        if (!bMatch)
                        {
                            Vec3b hsv = new Vec3b(0xFF, 0xFF, 0xFF);

                            filterImg.Set(y, x, hsv);

                        }
                    }
                }
            }

            viewImage = filterImg.Clone();// OpenCvSharp.Extensions.BitmapConverter.ToBitmap(filterImg);
            filterImg.Dispose();

            Image img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(viewImage);
            pictureBox.Image = img;

            //画像を表示する
            pictureBox.Invalidate();

            //            Image img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(filterImg);
            //           pictureBox.Image = img;
        }

        private void ListView1_SelectedIndexChanged_UsingItems(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection breakfast = this.listView1.SelectedItems;

            int index;

            foreach (ListViewItem item in breakfast)
            {
                string strTemp;
                strTemp = item.SubItems[0].Text;

                index = Convert.ToInt32(strTemp, 10);

                //listHouse[index].MakePolygon();

                KeyPointslistView.Items.Clear();

                if (nListViewMode == 1)
                {
                    for (int i = 0; i < listHouse[index].keypoints.Count(); i++)
                    {
                        string[] row = new string[2];

                        row[0] = listHouse[index].keypoints[i].X.ToString();
                        row[1] = listHouse[index].keypoints[i].Y.ToString();

                        KeyPointslistView.Items.Add(new ListViewItem(row));
                    }
                }
                else
                {
                    for (int i = 0; i < listRoad[index].keypoints.Count(); i++)
                    {
                        string[] row = new string[2];

                        row[0] = listRoad[index].keypoints[i].X.ToString();
                        row[1] = listRoad[index].keypoints[i].Y.ToString();

                        KeyPointslistView.Items.Add(new ListViewItem(row));
                    }
                }


                //描画先とするImageオブジェクトを作成する
                Bitmap canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
                //ImageオブジェクトのGraphicsオブジェクトを作成する
                Graphics g = Graphics.FromImage(canvas);

                //直線で接続する点の配列を作成
                Point2d[] ps;
                System.Drawing.Point[] ps2;
                if (nListViewMode == 1)
                {
                    ps = new Point2d[listHouse[index].keypoints.Count()];
                    ps2 = new System.Drawing.Point[listHouse[index].keypoints.Count()];
                }
                else
                {
                    ps = new Point2d[listRoad[index].keypoints.Count()];
                    ps2 = new System.Drawing.Point[listRoad[index].keypoints.Count()];

                }

                double minX = 10000;
                double minY = 10000;
                double maxX = 0;
                double maxY = 0;
                double xPer;
                double yPer;
                double width;
                double height;

                if (nListViewMode == 1)
                {
                    for (int i = 0; i < listHouse[index].keypoints.Count(); i++)
                    {
                        double x;
                        double y;

                        x = new GeoCoordinate(0, listHouse[index].BoxMin.X).GetDistanceTo(new GeoCoordinate(0, listHouse[index].keypoints[i].X));
                        y = new GeoCoordinate(listHouse[index].BoxMin.Y, 0).GetDistanceTo(new GeoCoordinate(listHouse[index].keypoints[i].Y, 0));

                        ps[i] = new Point2d(x, y);

                        if (minX > ps[i].X)
                        {
                            minX = ps[i].X;
                        }
                        if (minY > ps[i].Y)
                        {
                            minY = ps[i].Y;
                        }

                        if (maxX < ps[i].X)
                        {
                            maxX = ps[i].X;
                        }
                        if (maxY < ps[i].Y)
                        {
                            maxY = ps[i].Y;
                        }
                    }

                    width = maxX - minX;
                    height = maxY - minY;

                    xPer = (double)(pictureBox.Width - 100) / (double)width;
                    yPer = (double)(pictureBox.Height - 100) / (double)height;

                    for (int i = 0; i < listHouse[index].keypoints.Count(); i++)
                    {
                        ps2[i].X = (int)(((double)ps[i].X - minX) * Math.Min(xPer, yPer) + 50);
                        ps2[i].Y = (int)(((double)ps[i].Y - minY) * Math.Min(xPer, yPer) + 50);
                    }

                    //多角形を描画する
                    g.DrawPolygon(Pens.Black, ps2);
                }
                else
                {
                    for (int i = 0; i < listRoad[index].keypoints.Count(); i++)
                    {
                        double x;
                        double y;

                        x = new GeoCoordinate(0, listRoad[index].BoxMin.X).GetDistanceTo(new GeoCoordinate(0, listRoad[index].keypoints[i].X));
                        y = new GeoCoordinate(listRoad[index].BoxMin.Y, 0).GetDistanceTo(new GeoCoordinate(listRoad[index].keypoints[i].Y, 0));

                        ps[i] = new Point2d(x, y);

                        if (minX > ps[i].X)
                        {
                            minX = ps[i].X;
                        }
                        if (minY > ps[i].Y)
                        {
                            minY = ps[i].Y;
                        }

                        if (maxX < ps[i].X)
                        {
                            maxX = ps[i].X;
                        }
                        if (maxY < ps[i].Y)
                        {
                            maxY = ps[i].Y;
                        }
                    }

                    width = maxX - minX;
                    height = maxY - minY;

                    xPer = (double)(pictureBox.Width - 100) / (double)width;
                    yPer = (double)(pictureBox.Height - 100) / (double)height;

                    for (int i = 0; i < listRoad[index].keypoints.Count(); i++)
                    {
                        ps2[i].X = (int)(((double)ps[i].X - minX) * Math.Min(xPer, yPer) + 50);
                        ps2[i].Y = (int)(((double)ps[i].Y - minY) * Math.Min(xPer, yPer) + 50);
                    }

                    for (int i = 0; i < listRoad[index].keypoints.Count()-1; i++)
                    {
                        g.DrawLine(Pens.Black, ps2[i].X, ps2[i].Y, ps2[i+1].X, ps2[i+1].Y);
                    }
                }



                if (nListViewMode == 1)
                {
                    for (int i = 0; i < listHouse[index].sides.Count(); i++)
                    {
                        Side line = listHouse[index].sides[i];
                        float x1 = (float)(((double)line.Item0.X - minX) * Math.Min(xPer, yPer) + 50);
                        float y1 = (float)(((double)line.Item0.Y - minY) * Math.Min(xPer, yPer) + 50);
                        float x2 = (float)(((double)line.Item1.X - minX) * Math.Min(xPer, yPer) + 50);
                        float y2 = (float)(((double)line.Item1.Y - minY) * Math.Min(xPer, yPer) + 50);

                        g.DrawLine(Pens.Red, (float)x1, (float)y1, (float)x2, (float)y2);
                    }
                }
                else
                {
                    for (int i = 0; i < listRoad[index].sides.Count(); i++)
                    {
                        Side line = listRoad[index].sides[i];
                        float x1 = (float)(((double)line.Item0.X - minX) * Math.Min(xPer, yPer) + 50);
                        float y1 = (float)(((double)line.Item0.Y - minY) * Math.Min(xPer, yPer) + 50);
                        float x2 = (float)(((double)line.Item1.X - minX) * Math.Min(xPer, yPer) + 50);
                        float y2 = (float)(((double)line.Item1.Y - minY) * Math.Min(xPer, yPer) + 50);

                        g.DrawLine(Pens.Red, (float)x1, (float)y1, (float)x2, (float)y2);
                    }
                }

                //リソースを解放する
                g.Dispose();

                //PictureBox1に表示する
                pictureBox.Image = canvas;

                break;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Mat imageMat = new Mat(@"C:\Temp\test.bmp", ImreadModes.Color);   // OpenCvSharp 3.x

            loadImage = new Mat(imageMat.Height + Constants.mainViewPadding * 2, imageMat.Width + Constants.mainViewPadding * 2, MatType.CV_8UC3, Scalar.All(255));

            //処理領域を設定
            Rect roi = new Rect(Constants.mainViewPadding, Constants.mainViewPadding, imageMat.Width, imageMat.Height);

            loadImage[roi] = imageMat.Clone();

            viewImage = loadImage.Clone();
            Image img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(viewImage);
            pictureBox.Image = img;

            SetImgColorCount();

            //初期化
            drawRectangle = new Rectangle(0, 0, viewImage.Width, viewImage.Height);

            pictureBox.Invalidate();
        }



        //PictureBox1のMouseDownイベントハンドラ
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;

            System.Drawing.Point clickPt = new System.Drawing.Point(e.X - drawRectangle.X, e.Y - drawRectangle.Y);

            Vec3b px = viewImage.Get<Vec3b>(e.Y, e.X);


            if (px != new Vec3b(0xFF, 0xFF, 0xFF))
            {
                House houseInfo = GetSelectObject(e.X, e.Y);

                listHouse.Add(houseInfo);

                x_label.Text = houseInfo.RelPt.X.ToString();
                y_label.Text = houseInfo.RelPt.Y.ToString();
                w_label.Text = houseInfo.Width.ToString();
                h_label.Text = houseInfo.Height.ToString();

                Image subimg = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(houseInfo.ImgData);
                //subView.Image = subimg;

                //Mat dst = new Mat();
                //Cv2.DrawKeypoints(subviewImage, points, dst, new OpenCvSharp.Scalar(0, 0, 255));

                //Image subimg2 = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
                //subView.Image = subimg2;

                //ImageオブジェクトのGraphicsオブジェクトを作成する
                Graphics g = Graphics.FromImage(subimg);

                for (int l = 0; l < houseInfo.Count(); l++)
                {
                    //if (sides2.listSides[l].Count() == 1)
                    {
                        g.DrawLine(Pens.Black, (float)houseInfo.sides[l].Item0.X, (float)houseInfo.sides[l].Item0.Y, (float)houseInfo.sides[l].Item1.X, (float)houseInfo.sides[l].Item1.Y);
                    }
                }

                //リソースを解放する
                g.Dispose();
                //PictureBox1に表示する
                subView.Image = subimg;

                //画像を表示する
                //pictureBox.Invalidate();
            }
        }

        //PictureBox1のPaintイベントハンドラ
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            ;
        }

        private Rect GetRect(Mat data)
        {
            int minX, maxX, minY, maxY;
            int curX, curY;


            minX = 0;
            // minXを確定させる
            for (curX = 0; curX < data.Width; curX++)
            {
                for (curY = 0; curY < data.Height; curY++)
                {
                    Vec3b px = data.Get<Vec3b>(curY, curX);
                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        minX = curX;
                        break;
                    }
                }
                if (minX > 0) break;
    }

            maxX = 0;
            for (curX = data.Width - 1; curX >= 0; curX--)
            {
                for (curY = 0; curY < data.Height; curY++)
                {
                    Vec3b px = data.Get<Vec3b>(curY, curX);
                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        maxX = curX;
                        break;
                    }
                }
                if (maxX > 0) break;
            }

            maxY = 0;
            // minXを確定させる
            for (curY = 0; curY < data.Height; curY++)
            {
                for (curX = 0; curX < data.Width; curX++)
                {
                    Vec3b px = data.Get<Vec3b>(curY, curX);
                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        maxY = curY;
                        break;
                    }
                }
                if (maxY > 0) break;
            }

            minY = 0;
            // minXを確定させる
            for (curY = data.Height - 1; curY >= 0; curY--)
            {
                for (curX = 0; curX < data.Width; curX++)
                {
                    Vec3b px = data.Get<Vec3b>(curY, curX);
                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        minY = curY;
                        break;
                    }
                }
                if (minY > 0) break;
            }

            return new Rect(minX, maxY, maxX - minX, minY - maxY);
        }

        private House GetSelectObject(int startX, int startY)
        {
            House retObj;
            Mat work = viewImage.Clone();// new Mat(viewImage.Width, viewImage.Height, MatType.CV_8UC3, Scalar.All(255));
            Rect workRect;

            StackFrame CallStack = new StackFrame(1, true);

            //現在点からx+側にずらしてライン調べる
            int curX, curY;

            for (curY = 0; curY < work.Height; curY++)
            {
                for (curX = 0; curX < work.Width; curX++)
                {
                    work.Set<Vec3b>(curY, curX, new Vec3b(0xFF, 0xFF, 0xFF));
                }
            }

            workRect = GetRect(work);

            curY = startY;
            curX = startX;

            for (curX = startX; curX < viewImage.Width; curX++)
            {
                Vec3b px = viewImage.Get<Vec3b>(curY, curX);

                if (px == new Vec3b(0xFF, 0xFF, 0xFF))
                {
                    break;
                }
                else
                {
                    work.Set<Vec3b>(curY, curX, px);
                }
            }
            workRect = GetRect(work);
            for (curX = startX; curX > 0; curX--)
            {
                Vec3b px = viewImage.Get<Vec3b>(curY, curX);

                if (px == new Vec3b(0xFF, 0xFF, 0xFF))
                {
                    break;
                }
                else
                {
                    work.Set<Vec3b>(curY, curX, px);
                }
            }
            workRect = GetRect(work);

            //  一個ずつ上にずらす
            for (curY = startY - 1; curY >= 0; curY--)
            {
                int nUpdateCnt = 0;
                for (curX = 0; curX < viewImage.Width; curX++)
                {
                    Vec3b px = viewImage.Get<Vec3b>(curY, curX);

                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        //  一個下の行に隣接する点があれば値を同一オブジェクトとして更新する
                        if ((curX > 0 && work.Get<Vec3b>(curY + 1, curX - 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                         || work.Get<Vec3b>(curY + 1, curX) != new Vec3b(0xFF, 0xFF, 0xFF)
                         || (curX + 1 < work.Width && work.Get<Vec3b>(curY + 1, curX + 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                        )
                        {
                            work.Set<Vec3b>(curY, curX, px);

                            nUpdateCnt++;
                        }
                    }
                }
                //  探索終了
                if (nUpdateCnt == 0)
                {
                    break;
                }
            }
            workRect = GetRect(work);
            //  maxYから下にずらしていき、同一オブジェクトを更新していく
            for (curY = startY + 1; curY < viewImage.Height; curY++)
            {
                int nUpdateCnt = 0;
                for (curX = 0; curX < viewImage.Width; curX++)
                {
                    Vec3b px = viewImage.Get<Vec3b>(curY, curX);

                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        //  一個下の行に隣接する点があれば値を同一オブジェクトとして更新する
                        if ((curX > 0 && work.Get<Vec3b>(curY - 1, curX - 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                         || work.Get<Vec3b>(curY - 1, curX) != new Vec3b(0xFF, 0xFF, 0xFF)
                         || (curX + 1 < work.Width && work.Get<Vec3b>(curY - 1, curX + 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                        )
                        {
                            work.Set<Vec3b>(curY, curX, px);

                            nUpdateCnt++;
                        }
                    }
                }

                //  探索終了
                if (nUpdateCnt == 0)
                {
                    break;
                }
            }
            workRect = GetRect(work);
            //  minYは確定
            //  最後にminからmaxまで探索する
            //  一個ずつ上にずらす
            for (curY = workRect.Bottom - 1; curY >= 0; curY--)
            {
                int nUpdateCnt = 0;
                for (curX = 0; curX < viewImage.Width; curX++)
                {
                    Vec3b px = viewImage.Get<Vec3b>(curY, curX);

                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        //  一個下の行に隣接する点があれば値を同一オブジェクトとして更新する
                        if ((curX > 0 && work.Get<Vec3b>(curY + 1, curX - 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                         || work.Get<Vec3b>(curY + 1, curX) != new Vec3b(0xFF, 0xFF, 0xFF)
                         || (curX + 1 < work.Width && work.Get<Vec3b>(curY + 1, curX + 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                        )
                        {
                            work.Set<Vec3b>(curY, curX, px);

                            nUpdateCnt++;
                        }
                    }
                }
                //  探索終了
                if (nUpdateCnt == 0)
                {
                    break;
                }
            }
            workRect = GetRect(work);
            //  最後にminからmaxまで探索する
            //  一個ずつ上にずらす
            for (curY = workRect.Bottom - 1; curY >= 0; curY--)
            {
                int nUpdateCnt = 0;
                for (curX = viewImage.Width - 1; curX >= 0; curX--)
                {
                    Vec3b px = viewImage.Get<Vec3b>(curY, curX);

                    if (px != new Vec3b(0xFF, 0xFF, 0xFF))
                    {
                        //  一個下の行に隣接する点があれば値を同一オブジェクトとして更新する
                        if ((curX > 0 && work.Get<Vec3b>(curY + 1, curX - 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                         || work.Get<Vec3b>(curY + 1, curX) != new Vec3b(0xFF, 0xFF, 0xFF)
                         || (curX + 1 < work.Width && work.Get<Vec3b>(curY + 1, curX + 1) != new Vec3b(0xFF, 0xFF, 0xFF))
                        )
                        {
                            work.Set<Vec3b>(curY, curX, px);

                            nUpdateCnt++;
                        }
                    }
                }
                //  探索終了
                if (nUpdateCnt == 0)
                {
                    break;
                }
            }
            subviewRect = GetRect(work);

            workRect = new Rect(System.Math.Max(subviewRect.Left - Constants.subViewPadding, 0),
                    System.Math.Max(subviewRect.Top - Constants.subViewPadding, 0),
                    System.Math.Min(subviewRect.Right + Constants.subViewPadding, work.Width - 1) - System.Math.Max(subviewRect.Left - Constants.subViewPadding, 0),
                    System.Math.Min(subviewRect.Bottom + Constants.subViewPadding, work.Height - 1) - System.Math.Max(subviewRect.Top - Constants.subViewPadding, 0));

            retObj = new House(work[workRect].Clone(), subviewRect.Left - Constants.subViewPadding, subviewRect.Top - Constants.subViewPadding);

            return retObj;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<String,MapInfoJson> dic = new Dictionary<String, MapInfoJson>();

            for ( int i = 0;  i < listHouse.Count(); i++)
            {
                House data = listHouse[i];

                float minX = data.RelPt.X;
                float minY = data.RelPt.Y;
                if( minX < 0)
                {
                    minX -= 500;
                }
                else
                {
                    minX += 500;
                }
                if (minY < 0)
                {
                    minY -= 500;
                }
                else
                {
                    minY += 500;
                }
                int indexX = (int)(minX / 1000);
                int indexY = (int)(minY / 1000);
                string strIndex = indexX.ToString() + "x" + indexY.ToString();

                if ( !(dic.ContainsKey(strIndex) )) {
                    dic.Add(strIndex, new MapInfoJson() );
                }

                dic[strIndex].Add(data.build);
            }

            foreach (var key in dic.Keys)
            {
                using (var ms = new MemoryStream())
                using (var sr = new StreamReader(ms))
                {
                    var serializer = new DataContractJsonSerializer(typeof(MapInfoJson));
                    serializer.WriteObject(ms, dic[key]);
                    ms.Position = 0;

                    var json = sr.ReadToEnd();

                    string strFName = "mapinfo_" + key + ".json";
                    string strPath = @"C:/FastSource/UnityProjects/Dive_Type1/Assets/Json/" + strFName;
                    System.IO.File.WriteAllText(strPath, json);
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //shp.Shpfile shpfile = new shp.Shpfile(@"C:\FastSource\AnalyzeShp\Data\SampleShp\japan_ver81.shp");
            shp.Shpfile shpfile = new shp.Shpfile(@"C:\Users\Kirin\Downloads\planet_139.441,35.572_139.998,35.853-shp\shape\buildings.shp");
            //shp.Shpfile shpfile = new shp.Shpfile(@"D:\Source\MapImage\Data\SampleShp\japan_ver81.shp");

            

            for (int i = 0; i < shpfile.Contents.listContents.Count(); i++)
            {
                for( int j = 0; j < shpfile.Contents.listContents[i].listParts.Count(); j++)
                {
                    int start;
                    int end;

                    start = shpfile.Contents.listContents[i].listParts[j];
                    if (j + 1 == shpfile.Contents.listContents[i].listParts.Count())
                    {
                        end = shpfile.Contents.listContents[i].listPoints.Count() -1;
                    }
                    else {
                        end = shpfile.Contents.listContents[i].listParts[j + 1] - 1;
                    }
                    //  仕様上end　はstartと同じ座標になるのでendを一個前にずらす
                    end--;

                    House obj = new House();

                    obj.SetRect(
                        shpfile.Contents.listContents[i].Box[0],
                        shpfile.Contents.listContents[i].Box[1],
                        shpfile.Contents.listContents[i].Box[2],
                        shpfile.Contents.listContents[i].Box[3]
                        );

                    for ( int k = start; k <= end; k++)
                    {

                        shp.Point pt;
                        pt = shpfile.Contents.listContents[i].listPoints[k];

                        //var x = new GeoCoordinate(0, obj.BoxMin.X).GetDistanceTo(new GeoCoordinate(0, pt.x));
                        //var y = new GeoCoordinate(obj.BoxMin.Y, 0).GetDistanceTo(new GeoCoordinate(pt.y, 0));

                        obj.AddKeyPoint(pt.x, pt.y);
                    }

                    if (obj.keypoints.Count() > 2)
                    {
                        obj.Long2Dist();

                        obj.CalcSize();

                        obj.MakePolygon();

                        listHouse.Add(obj);
                    }
                }
            }

            listView1.Items.Clear();
            nListViewMode = 1;
            //foreach (var v in listHouse)
            for ( int i = 0; i < listHouse.Count(); i++ )
            {
                string[] row = new string[6];

                row[0] = i.ToString();
                row[1] = listHouse[i].keypoints.Count().ToString();
                row[2] = listHouse[i].BoxMin.X.ToString();
                row[3] = listHouse[i].BoxMin.Y.ToString();
                row[4] = listHouse[i].BoxMax.X.ToString();
                row[5] = listHouse[i].BoxMax.Y.ToString();

                listView1.Items.Add(new ListViewItem(row));

                if( i > 3000)
                {
                    break;
                }
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //shp.Shpfile shpfile = new shp.Shpfile(@"C:\FastSource\AnalyzeShp\Data\SampleShp\japan_ver81.shp");
            shp.Shpfile shpfile = new shp.Shpfile(@"C:\Users\Kirin\Downloads\planet_139.441,35.572_139.998,35.853-shp\shape\roads.shp");
            //shp.Shpfile shpfile = new shp.Shpfile(@"D:\Source\MapImage\Data\SampleShp\japan_ver81.shp");



            for (int i = 0; i < shpfile.ContentPolyline.listContents.Count(); i++)
            {
                for (int j = 0; j < shpfile.ContentPolyline.listContents[i].listParts.Count(); j++)
                {
                    int start;
                    int end;

                    start = shpfile.ContentPolyline.listContents[i].listParts[j];
                    if (j + 1 == shpfile.ContentPolyline.listContents[i].listParts.Count())
                    {
                        end = shpfile.ContentPolyline.listContents[i].listPoints.Count() - 1;
                    }
                    else
                    {
                        end = shpfile.ContentPolyline.listContents[i].listParts[j + 1] - 1;
                    }
                    //  仕様上end　はstartと同じ座標になるのでendを一個前にずらす
                    end--;

                    Road obj = new Road();

                    obj.SetRect(
                        shpfile.ContentPolyline.listContents[i].Box[0],
                        shpfile.ContentPolyline.listContents[i].Box[1],
                        shpfile.ContentPolyline.listContents[i].Box[2],
                        shpfile.ContentPolyline.listContents[i].Box[3]
                        );

                    for (int k = start; k <= end; k++)
                    {

                        shp.Point pt;
                        pt = shpfile.ContentPolyline.listContents[i].listPoints[k];

                        //var x = new GeoCoordinate(0, obj.BoxMin.X).GetDistanceTo(new GeoCoordinate(0, pt.x));
                        //var y = new GeoCoordinate(obj.BoxMin.Y, 0).GetDistanceTo(new GeoCoordinate(pt.y, 0));

                        obj.AddKeyPoint(pt.x, pt.y);
                    }

                    if (obj.keypoints.Count() > 1)
                    {
                        obj.Long2Dist();

                        obj.CalcSize();

                        obj.MakeRoadLine();

                        listRoad.Add(obj);
                    }
                }
            }

            listView1.Items.Clear();
            nListViewMode = 2;
            //foreach (var v in listHouse)
            for (int i = 0; i < listRoad.Count(); i++)
            {
                string[] row = new string[6];

                row[0] = i.ToString();
                row[1] = listRoad[i].keypoints.Count().ToString();
                row[2] = listRoad[i].BoxMin.X.ToString();
                row[3] = listRoad[i].BoxMin.Y.ToString();
                row[4] = listRoad[i].BoxMax.X.ToString();
                row[5] = listRoad[i].BoxMax.Y.ToString();

                listView1.Items.Add(new ListViewItem(row));

                if (i > 3000)
                {
                    break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dictionary<String, MapInfoJson> dic = new Dictionary<String, MapInfoJson>();

            for (int i = 0; i < listRoad.Count(); i++)
            {
                Road data = listRoad[i];

                float minX = data.RelPt.X;
                float minY = data.RelPt.Y;
                if (minX < 0)
                {
                    minX -= 500;
                }
                else
                {
                    minX += 500;
                }
                if (minY < 0)
                {
                    minY -= 500;
                }
                else
                {
                    minY += 500;
                }
                int indexX = (int)(minX / 1000);
                int indexY = (int)(minY / 1000);
                string strIndex = indexX.ToString() + "x" + indexY.ToString();

                if (!(dic.ContainsKey(strIndex)))
                {
                    dic.Add(strIndex, new MapInfoJson());
                }

                dic[strIndex].Add(data.build);
            }

            foreach (var key in dic.Keys)
            {
                using (var ms = new MemoryStream())
                using (var sr = new StreamReader(ms))
                {
                    var serializer = new DataContractJsonSerializer(typeof(MapInfoJson));
                    serializer.WriteObject(ms, dic[key]);
                    ms.Position = 0;

                    var json = sr.ReadToEnd();

                    string strFName = "roadinfo_" + key + ".json";
                    string strPath = @"C:/FastSource/UnityProjects/Dive_Type1/Assets/Json/" + strFName;
                    System.IO.File.WriteAllText(strPath, json);
                }
            }
        }
    }
}


#if false
[DataContract]
public class Person
{
    [DataMember]
    public int ID { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public IDictionary<string, string> Attributes { get; private set; }

    public Person()
    {
        this.Attributes = new Dictionary<string, string>();
    }
}
#endif
static class Constants
{
    public const int mainViewPadding = 20;
    public const int subViewPadding = 10;

}


namespace DistortImage
{
    public partial class FormMain : Form
    {
        protected Bitmap bitmap;
        protected List<ScrollBar> scrollBars = new();
        protected List<TextBox> textBoxes = new();
        public FormMain()
        {
            InitializeComponent();

            this.pictureBoxShow.Image = this.bitmap = Image.FromFile("cb.png") as Bitmap;

            this.scrollBars.Add(this.vScrollBar0);
            this.scrollBars.Add(this.vScrollBar1);
            this.scrollBars.Add(this.vScrollBar2);
            this.scrollBars.Add(this.vScrollBar3);
            this.scrollBars.Add(this.vScrollBar4);

            this.textBoxes.Add(this.textBox_c0);
            this.textBoxes.Add(this.textBox_c1);
            this.textBoxes.Add(this.textBox_c2);
            this.textBoxes.Add(this.textBox_c3);
            this.textBoxes.Add(this.textBox_c4);

            foreach (var sb in this.scrollBars)
            {
                sb.Value = (int)(this.consts[sb.TabIndex] * 1000.0 / 10.0);
            }
        }
        protected double[] consts = new double[5]{
            1.0,//-0.9998,//scale factor
            -4.3932,
            +1.4327,
            -2.8526,
            +9.8223
        };

        private void vScrollBars_ValueChanged(object sender, EventArgs e)
        {
            if (sender is ScrollBar sb)
            {
                int i = this.scrollBars.IndexOf(sb);
                if (i != -1)
                {
                    this.consts[i] = sb.Value / 1000.0 * 10.0;
                    this.UpdateImage();
                }
            }
        }
        public struct PointD
        {
            public double X;
            public double Y;
            public PointD(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }
        private void UpdateImage()
        {
            for (int i = 0; i < consts.Length; i++)
            {
                this.textBoxes[i].Text = this.consts[i].ToString();
            }
            
            var source_image = this.bitmap;
            if (source_image != null)
            {
                var dest_image = new Bitmap(source_image);

                Point lenscenter = new Point(source_image.Width / 2, source_image.Height / 2);//��ͷ������ͼ���е�λ��  
                Point src_a, src_b, src_c, src_d;//a��b��c��d�ĸ�����
                                                 //��������
                double r;//����ǰ���ص����ͷ���ĵľ���
                double s;//���������ص����ͷ���ĵľ���
                PointD mCorrectPoint;//�����������
                double distance_to_a_x;
                double distance_to_a_y;//������ĵ�ͱ߽�ľ���

                double c0 = consts[0];
                double c1 = consts[1];
                double c2 = consts[2]; //inner outer
                double c3 = consts[3];
                double c4 = consts[4];

                for (int y = 0; y < source_image.Height; y++)//����������,Ҫע��OpenCV��RGB�Ĵ洢˳��ΪGBR    
                    for (int x = 0; x < source_image.Width; x++)//ʾ��Ϊ���ȵ���    
                    {
                        r = Math.Sqrt(
                            (y - lenscenter.Y) * (y - lenscenter.Y)
                            + (x - lenscenter.X) * (x - lenscenter.X));
                        s = c0
                            + c1 * 1e-4 * r//pow(r,1) 
                            + c2 * 1e-6 * r * r
                            + c3 * 1e-9 * r * r * r
                            + c4 * 1e-13 * r * r * r * r;//����  
                        if (s == 0) return;
                        s = Math.Abs(s);

                        mCorrectPoint = new PointD(
                            (x - lenscenter.X) / s + lenscenter.X,
                            (y - lenscenter.Y) / s + lenscenter.Y);
                        //Խ���ж�
                        if (mCorrectPoint.Y < 0 || mCorrectPoint.Y >= source_image.Height - 1)
                        {
                            continue;
                        }
                        if (mCorrectPoint.X < 0 || mCorrectPoint.X >= source_image.Width - 1)
                        {
                            continue;
                        }
                        src_a = new Point((int)mCorrectPoint.X, (int)mCorrectPoint.Y);
                        src_b = new Point(src_a.X + 1, src_a.Y);
                        src_c = new Point(src_a.X, src_a.Y + 1);
                        src_d = new Point(src_a.X + 1, src_a.Y + 1);
                        distance_to_a_x = mCorrectPoint.X - src_a.X;//��ԭͼ������a���ˮƽ����    
                        distance_to_a_y = mCorrectPoint.Y - src_a.Y;//��ԭͼ������a��Ĵ�ֱ����    	

                        double R =
                            source_image.GetPixel(src_a.X, src_a.Y).R * (1 - distance_to_a_x) * (1 - distance_to_a_y) +
                            source_image.GetPixel(src_b.X, src_b.Y).R * distance_to_a_x * (1 - distance_to_a_y) +
                            source_image.GetPixel(src_c.X, src_c.Y).R * distance_to_a_y * (1 - distance_to_a_x) +
                            source_image.GetPixel(src_d.X, src_d.Y).R * distance_to_a_y * distance_to_a_x;

                        double G =
                            source_image.GetPixel(src_a.X, src_a.Y).G * (1 - distance_to_a_x) * (1 - distance_to_a_y) +
                            source_image.GetPixel(src_b.X, src_b.Y).G * distance_to_a_x * (1 - distance_to_a_y) +
                            source_image.GetPixel(src_c.X, src_c.Y).G * distance_to_a_y * (1 - distance_to_a_x) +
                            source_image.GetPixel(src_d.X, src_d.Y).G * distance_to_a_y * distance_to_a_x;

                        double B =
                            source_image.GetPixel(src_a.X, src_a.Y).B * (1 - distance_to_a_x) * (1 - distance_to_a_y) +
                            source_image.GetPixel(src_b.X, src_b.Y).B * distance_to_a_x * (1 - distance_to_a_y) +
                            source_image.GetPixel(src_c.X, src_c.Y).B * distance_to_a_y * (1 - distance_to_a_x) +
                            source_image.GetPixel(src_d.X, src_d.Y).B * distance_to_a_y * distance_to_a_x;

                        Color c = Color.FromArgb((int)R, (int)G, (int)B);

                        dest_image.SetPixel(x, y, c);

                        this.pictureBoxShow.Image = dest_image;
                    }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            var any = false;
            for (int i = 0;i<consts.Length;i++)
            {
                var text = this.textBoxes[i].Text;
                if(double.TryParse(text, out var val))
                {
                    this.consts[i] = val;
                    any = true;
                }
            }
            if (any)
            {
                this.UpdateImage();
            }
        }
    }
}
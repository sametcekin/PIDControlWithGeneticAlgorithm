using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PIDControlWithGeneticAlgorithm.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            kontrolEdilecekSistem = comboBoxKontrolSistemi.Text;
        }

        private void comboBoxKontrolSistemi_SelectedIndexChanged(object sender, EventArgs e)
        {
            kontrolEdilecekSistem = comboBoxKontrolSistemi.Text;
        }
        double pidCikisi = 0;
        double referansGirisi = 0;
        double referansCikisi = 0;
        double kp = 0;
        double ki = 0;
        double kd = 0;
        double hata = 0;
        double oncekiHata = 0;
        double integral = 0;
        double oncekiIntegral = 0;
        double turev = 0;
        double adimAraligi = 0;
        int adimsayisi = 0;
        bool sistemOturdu = false;

        private void buttonHesapla_Click(object sender, EventArgs e)
        {
            degiskenSifirla();

            referansGirisi = Convert.ToDouble(textBoxReferans.Text);
            kp = Convert.ToDouble(textBoxKp.Text);
            ki = Convert.ToDouble(textBoxKi.Text);
            kd = Convert.ToDouble(textBoxKd.Text);
            adimAraligi = Convert.ToDouble(textBoxAdimAraligi.Text);
            adimsayisi = Convert.ToInt32(textBoxAdimSayisi.Text);
            textBoxt.Text = (adimAraligi * adimsayisi).ToString();
            textBoxSistemGirisi.Text = textBoxReferans.Text;
            labelHataDeger.Text = labelSaniye.Text = "--";

            listBoxReferansCikisDegerleri.Items.Clear();
            listBoxHataDegerleri.Items.Clear();
            dataX = new double[adimsayisi];
            dataY = new double[adimsayisi];
            for (int i = 0; i < adimsayisi; i++)
            {
                hata = referansGirisi - referansCikisi;
                integral = ((hata + oncekiHata) * adimAraligi) / 2;
                turev = (hata - oncekiHata) / adimAraligi;
                pidCikisi = (kp * hata) + (ki * (oncekiIntegral + integral)) + (kd * turev);
                sistemCikisiHesapla();
                oncekiHata = hata;
                oncekiIntegral += integral;

                listBoxHataDegerleri.Items.Add(((i + 1).ToString()) + ".Adım= " + (hata.ToString()));
                listBoxReferansCikisDegerleri.Items.Add(((i + 1).ToString()) + ".Adım= " + (referansCikisi.ToString()));
                textBoxHata.Text = labelHataDeger.Text = hata.ToString();
                textBoxIntegral.Text = integral.ToString();
                textBoxTurev.Text = turev.ToString();
                textBoxSistemCikisi.Text = referansCikisi.ToString();

                if (textBoxHata.Text == "0" && sistemOturdu == false)
                {
                    sistemOturdu = true;
                    labelSaniye.Text = ((i + 2) * adimAraligi).ToString();
                }
                dataX[i]= Convert.ToDouble(i * adimAraligi);
                dataY[i]= referansCikisi;
                wpfPlot1.plt.Clear();
                wpfPlot1.plt.PlotScatter(dataX, dataY);
                wpfPlot1.Render();
            }
        }
        double[] dataX;
        double[] dataY;
        string kontrolEdilecekSistem;
        string carprazlamaSecim;
        void sistemCikisiHesapla()
        {
            if (kontrolEdilecekSistem == "Motor Hız Kontrolü")
            {
                if (pidCikisi < 0)
                    pidCikisi = 0;
                if (pidCikisi <= 10 && pidCikisi >= 0)
                    referansCikisi = pidCikisi * 5;
                else if (pidCikisi > 10)
                    referansCikisi = 50;

            }
            if (kontrolEdilecekSistem == "Lamba Parlaklık Kontrolü")
            {
                if (pidCikisi < 0)
                    pidCikisi = 0;
                referansCikisi = Math.Pow(pidCikisi, 2) / 5;
            }
        }
        void degiskenSifirla()
        {
            pidCikisi = 0;
            referansGirisi = 0;
            referansCikisi = 0;
            kp = 0;
            ki = 0;
            kd = 0;
            hata = 0;
            oncekiHata = 0;
            integral = 0;
            oncekiIntegral = 0;
            turev = 0;
            adimAraligi = 0;
            adimsayisi = 0;
            sistemOturdu = false;
            hataToplam = 0;
            //foreach (Control item in groupBox2.Controls)
            //{
            //    if (item is TextBox)
            //    {
            //        TextBox tbox = (TextBox)item;
            //        tbox.Text = "0";
            //    }
            //}
        }
        private void buttonPIDBul_Click(object sender, EventArgs e)
        {
            if (comboBoxKontrolSistemi.Text == "Motor Hız Kontrolü")
            {
                Xmin = new double[3] { 0, 0, 0 };
                Xmax = new double[3] { 0.1, 3, 0.001 };
            }
            else if (comboBoxKontrolSistemi.Text == "Lamba Parlaklık Kontrolü")
            {
                Xmin = new double[3] { 0, 0, 0 };
                Xmax = new double[3] { 0.01, 1, 0.01 };
            }
            carprazlamaSecim = comboBoxCSecim.Text;


            GN = Convert.ToInt32(textBoxJenerasyonSayisi.Text);
            PN = Convert.ToInt32(textBoxPopulasyonSayisi.Text);
            CO = Convert.ToInt32(textBoxCarprazlamaOrani.Text);
            MO = Convert.ToInt32(textBoxMutasyonOrani.Text);
            degiskenSifirla();
            GenetikAlgoritma();
        }

        Random objRandom;
        ArrayList index;

        int GN, PN;
        double CO, MO;
        double[] Xmin;
        double[] Xmax;
        double BestFitness = 0.0;
        int PartNumber = 0;
        double[,] Population;
        double[] Fitness;
        double minFitness = 0;
        int minIndex = 0;
        int BestGN = 0;
        double[] SelFitness;
        double[] Probability;
        double[] Cumulative;
        double[] RandomSNum;
        double[,] DogalPopulation;
        double cumsum = 0.0;
        double[] RandomCNum;
        int CONum = 0;
        void GenetikAlgoritma()
        {
            objRandom = new Random();
            PartNumber = Xmin.Length;
            Population = new double[PN, 3];
            Fitness = new double[PN];
            SelFitness = new double[PN];
            Probability = new double[PN];
            Cumulative = new double[PN];
            RandomSNum = new double[PN];
            DogalPopulation = new double[PN, 3];
            RandomCNum = new double[PN];
            index = new ArrayList();
            CONum = 0;
            //Başlangıç popülasyonunun oluşturulması
            for (int i = 0; i < PN; i++)
            {
                Population[i, 0] = Xmin[0] + objRandom.NextDouble() * (Xmax[0] - Xmin[0]);
                Population[i, 1] = Xmin[1] + objRandom.NextDouble() * (Xmax[1] - Xmin[1]);
                Population[i, 2] = Xmin[2] + objRandom.NextDouble() * (Xmax[2] - Xmin[2]);
            }
            //Uygunluk değerlerinin hesaplanması
            for (int i = 0; i < PN; i++)
            {
                kp = Population[i, 0];
                ki = Population[i, 1];
                kd = Population[i, 2];

                Fitness[i] = hataDegeri(kp, ki, kd);
            }
            minFitness = Fitness.Min();
            minIndex = Array.IndexOf(Fitness, minFitness);
            BestFitness = minFitness;
            for (int i = 0; i < GN; i++)
            {
                if (minFitness < BestFitness)
                {
                    BestFitness = minFitness;
                    BestGN = i;
                }
                //Doğal Seçim işlemi
                for (int j = 0; j < PN; j++)
                {
                    SelFitness[j] = Fitness[j] + 1.1;//SelFitness = Fitness + 1.1; 
                }
                for (int j = 0; j < PN; j++)
                {
                    SelFitness[j] = 1 / SelFitness[j];//SelFitness = 1 ./ SelFitness;
                }
                for (int j = 0; j < PN; j++)
                {
                    Probability[j] = SelFitness[j] / SelFitness.Sum();//Probability = SelFitness / sum(SelFitness);

                }
                cumsum = 0.0;
                for (int j = 0; j < PN; j++)
                {
                    for (int k = 0; k <= j; k++)
                    {
                        cumsum += Probability[k];
                    }
                    Cumulative[j] = cumsum;//Cumulative = cumsum(Probability);
                }
                for (int j = 0; j < PN; j++)
                {
                    RandomSNum[j] = objRandom.NextDouble();// RandomSNum = rand(PN, 1);
                }
                for (int j = 0; j < PN; j++)
                {
                    index.Clear();
                    for (int k = 0; k < PN; k++)
                    {
                        if (RandomSNum[k] < Cumulative[j])
                        {
                            index.Add(k);
                        }
                    }
                    foreach (int item in index)
                    {
                        RandomSNum[item] = j;//
                    }
                }
                for (int j = 0; j < PN; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        DogalPopulation[j, k] = Population[j, k];
                    }
                }
                for (int j = 0; j < PN; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Population[j, k] = DogalPopulation[Convert.ToInt32(RandomSNum[j]), k];//Population = Population(RandomSNum, :);
                    }
                }
                //Caprazlama islemi                
                for (int j = 0; j < PN; j++)
                {
                    RandomCNum[j] = objRandom.NextDouble();//RandomCNum = rand(PN, 1);
                }
                index.Clear();
                for (int j = 0; j < PN; j++)
                {
                    if (RandomCNum[j] < (CO / 100))
                        index.Add(j);//index = find(RandomCNum < (CO/100));
                }
                CONum = index.Count;//CONum = length(index);
                while (CONum < Math.Ceiling(PN * (CO / 100)))
                {
                    int a = Convert.ToInt32(Math.Round(objRandom.NextDouble() * PN));
                    if (a != 0 & a != 100)
                    {
                        index.Add(a);
                        CONum = index.Count;
                    }
                }
                while (CONum > Math.Ceiling(PN * (CO / 100)))
                {
                    index.RemoveAt(CONum - 1);
                    CONum = index.Count;
                }
                while (CONum % 2 == 1)
                {
                    int a = Convert.ToInt32(Math.Round(objRandom.NextDouble() * PN));
                    if (a != 0 & a != 100)
                    {
                        index.Add(a);
                        CONum = index.Count;
                    }
                }

                int[] indexdizi = new int[index.Count + 1];
                int say = 1;
                foreach (int item in index)
                {
                    indexdizi[say] = item;
                    say++;
                }
                for (int j = 1; j < (CONum + 1) / 2; j++)
                {
                    int CrossPoint = Convert.ToInt32(Math.Ceiling(objRandom.NextDouble() * (PartNumber - 1)));
                    double Beta = objRandom.NextDouble();
                    if (carprazlamaSecim == "Tek Nokta")
                    {
                        double Param = Population[indexdizi[2 * j - 1], CrossPoint];
                        Population[indexdizi[2 * j - 1], CrossPoint] = Population[indexdizi[2 * j], CrossPoint];
                        Population[indexdizi[2 * j], CrossPoint] = Param;
                    }
                    else if (carprazlamaSecim == "Çoklu Nokta")
                    {
                        double Param1 = Beta * Population[indexdizi[2 * j - 1], CrossPoint] + (1 - Beta) * (Population[indexdizi[2 * j], CrossPoint]);
                        double Param2 = (1 - Beta) * Population[indexdizi[2 * j - 1], CrossPoint] + Beta * (Population[indexdizi[2 * j], CrossPoint]);
                        Population[indexdizi[2 * j - 1], CrossPoint] = Param1;
                        Population[indexdizi[2 * j], CrossPoint] = Param2;
                    }

                }
                // Mutasyon islemi
                double[] RandomMNum = new double[PN * PartNumber];
                index.Clear();
                if (i != GN)
                {
                    for (int j = 0; j < RandomMNum.Length; j++)
                    {
                        RandomMNum[j] = objRandom.NextDouble();
                    }
                    for (int j = 0; j < RandomMNum.Length; j++)
                    {
                        if (RandomMNum[j] < (MO / 100))
                        {
                            index.Add(j);
                        }
                    }

                    int MONum = index.Count;
                    int[] indexdizi2 = new int[index.Count];
                    int say2 = 0;
                    foreach (int item in index)
                    {
                        indexdizi2[say2] = item;
                        say2++;
                    }
                    for (int j = 0; j < MONum; j++)
                    {
                        Population[Convert.ToInt32(Math.Ceiling(Convert.ToDouble(indexdizi2[j] / PartNumber))), (indexdizi2[j] % PartNumber)] = Xmin[(indexdizi2[j] % PartNumber)] + objRandom.NextDouble() * (Xmax[(indexdizi2[j] % PartNumber)] - Xmin[(indexdizi2[j] % PartNumber)]);
                    }
                }
                //Uygunluk değerlerinin hesaplanması 
                for (int j = 0; j < PN; j++)
                {
                    kp = Population[j, 0];
                    ki = Population[j, 1];
                    kd = Population[j, 2];

                    Fitness[j] = hataDegeri(kp, ki, kd);
                }
                minFitness = Fitness.Min();
                minIndex = Array.IndexOf(Fitness, minFitness);
                BestFitness = minFitness;
            }

            //1
            double[] deger = new double[GN];

            //2
            textBoxKp.Text = kp.ToString();
            textBoxKi.Text = ki.ToString();
            textBoxKd.Text = kd.ToString();
            textBoxBestFitnessDegeri.Text = BestFitness.ToString();
        }

        double hataToplam = 0;
        double hataDegeri(double kp, double ki, double kd)
        {
            referansGirisi = Convert.ToDouble(textBoxReferans.Text);
            adimAraligi = Convert.ToDouble(textBoxAdimAraligi.Text);
            adimsayisi = Convert.ToInt32(textBoxAdimSayisi.Text);
            for (int i = 0; i < adimsayisi; i++)
            {
                hata = referansGirisi - referansCikisi;
                integral = ((hata + oncekiHata) * adimAraligi) / 2;
                turev = (hata - oncekiHata) / adimAraligi;
                pidCikisi = (kp * hata) + (ki * (oncekiIntegral + integral)) + (kd * turev);
                sistemCikisiHesapla();
                oncekiHata = hata;
                oncekiIntegral += integral;
                hataToplam += Math.Pow(hata, 2);
            }
            return hataToplam;
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Columns_Sections
{
    /// <summary>
    /// Interaction logic for ventana.xaml
    /// </summary>
    public partial class ventana : Window
    {
        double cmin;
        double cmax;
        double cvar;
        double cuantia;
        int acerodir2;
        int acerodir3;
        int numerodebarrasminimo2;
        int numerodebarrasminimo3;
        int numerodebarrasmaximo2;
        int numerodebarrasmaximo3;
        double cuantiar;
        double fr = 6;                          //relacion ancho alto
        double fce = 1;                         //factor de escala
        double espmax = 20;
        double espmin = 2.54;
        List<double> diam = new List<double>();                           // diametros de barras
        List<double> areas = new List<double>();                           // areas de barras
        List<string> rebarsize = new List<string>();                 // nombre de las barras
        List<string> sec = new List<string>();                       // secciones creadas
        string sectname;                        // secciones nombres
        int i = 0;                              //contador

        protected cPlugin ParentPluginObject;
        protected ETABS2016.cSapModel mySapModel;
        protected ETABS2016.cPluginCallback ISapPlugin;
        protected Datos data;
        public int ret = 0;


        public void setParentPluginObject(cPlugin inParentPluginObject)
        {
            ParentPluginObject = inParentPluginObject;

        }
        public void setSapModel(ETABS2016.cSapModel inSapModel, ETABS2016.cPluginCallback inISapPlugin, Datos Idata)
        {
            mySapModel = inSapModel;
            ISapPlugin = inISapPlugin;
            data = Idata;
            
        }
        public int typerebar(string seccion, int tipo, ETABS2016.cSapModel mySapModel)
        {
            ret = mySapModel.PropFrame.GetTypeRebar //Lee nombre de la seccion y decide si se diseña como viga o columna
               (
                   seccion,
                   ref tipo
               );
            return tipo;
        }


        public ventana()
        {         
            
            InitializeComponent();


        }

        public void button_Click(object sender, RoutedEventArgs e)
        {

            if (IsntNumeric(Cmin.Text))
            {
                Cmin.Text = "1";
            }
            if (IsntNumeric(Cmax.Text))
            {
                Cmax.Text = "6";
            }
            if (IsntNumeric(Cvariacion.Text))
            {
                Cvariacion.Text = "1";
            }

            cmin = double.Parse(Cmin.Text)/100;
            cmax = double.Parse(Cmax.Text)/100;
            cvar = double.Parse(Cvariacion.Text)/100;
            diam.Add(data.rebardata.Diameters[Barramenor.SelectedIndex]);
            diam.Add(data.rebardata.Diameters[Barramayor.SelectedIndex]);
            areas.Add(data.rebardata.Areas[Barramenor.SelectedIndex]);
            areas.Add(data.rebardata.Areas[Barramayor.SelectedIndex]);
            rebarsize.Add(data.rebardata.MyName[Barramenor.SelectedIndex]);
            rebarsize.Add(data.rebardata.MyName[Barramayor.SelectedIndex]);
            espmax = espmax * fce;
            espmin = espmin * fce;
            foreach (string name in data.frame_Section.MyName)
            {
                //MessageBox.Show(name);
                int i = 0;
                data.column_Rebar.mytipe = typerebar(name, data.column_Rebar.mytipe, mySapModel);
                //MessageBox.Show(data.column_Rebar.mytipe.ToString());
                // Obtiene el armado de las columnas rectangulares
                //MessageBox.Show(data.column_Rebar.mytipe.ToString());
                if (data.column_Rebar.mytipe == 1)
                {
                    //MessageBox.Show(name);
                    ret = mySapModel.PropFrame.GetRebarColumn
                        (
                             name,
                         ref data.column_Rebar.MatPropLong,
                         ref data.column_Rebar.MatPropConfine,
                         ref data.column_Rebar.Pattern,
                         ref data.column_Rebar.ConfineType,
                         ref data.column_Rebar.Cover,
                         ref data.column_Rebar.NumberCBars,
                         ref data.column_Rebar.NumberR3Bars,
                         ref data.column_Rebar.NumberR2Bars,
                         ref data.column_Rebar.RebarSize,
                         ref data.column_Rebar.TieSize,
                         ref data.column_Rebar.TieSpacingLongit,
                         ref data.column_Rebar.Number2DirTieBars,
                         ref data.column_Rebar.Number3DirTieBars,
                         ref data.column_Rebar.ToBeDesigned

                        );
                    ret = mySapModel.PropRebar.GetRebarProps(data.column_Rebar.TieSize, ref data.column_Rebar.REstarea, ref data.column_Rebar.REstdiam);

                    if (data.column_Rebar.Pattern == 1)
                    {//rectangular
                        ret = mySapModel.PropFrame.GetRectangle(name,
                            ref data.frame_Section.file,
                            ref data.frame_Section.matprop,
                            ref data.frame_Section.t3,
                            ref data.frame_Section.t2,
                            ref data.frame_Section.color,
                            ref data.frame_Section.note,
                            ref data.frame_Section.guid);

                        ciclocuantiacolumnarectangular(data.frame_Section.t3, data.frame_Section.t2, name);
                        

                    }
                    else
                    {//circular
                        ret = mySapModel.PropFrame.GetCircle(name,
                            ref data.frame_Section.file,
                            ref data.frame_Section.matprop,
                            ref data.frame_Section.t3,
                            ref data.frame_Section.color,
                            ref data.frame_Section.note,
                            ref data.frame_Section.guid);
                        ciclocuantiacolumnacircular(data.frame_Section.t3, name);
                        


                    }

                }
                else
                {
                    sec.Add(name);
                }
                
                
            }
            data.frame_Section.MyName = null;
            ret = mySapModel.PropFrame.GetNameList(ref data.frame_Section.NumberNames, ref data.frame_Section.MyName);
            foreach (string name in data.frame_Section.MyName)
            {
                if (!sec.Contains(name))
                    ret = mySapModel.PropFrame.Delete(name);
            }
            
                MessageBox.Show("ok");
        }

        public void ciclocuantiacolumnacircular(double diametro, string name)
        {
            i = 0;
            numerodebarrasminimo2 = (int)Math.Ceiling((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmax + diam[i]));
            numerodebarrasmaximo2 = (int)Math.Floor((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmin + diam[i]));
            cmin = 0.01;
            while (cmin <= cmax)
            {

                while (i < areas.Count)
                {
                    cuantia = diametro * diametro * cmin * Math.PI / 4;
                    acerodir2 = (int)Math.Ceiling(cuantia / areas[i]);
                    if (acerodir2 < numerodebarrasminimo2)
                        acerodir2 = numerodebarrasminimo2;
                    if (acerodir2 > numerodebarrasmaximo2)
                    {
                        if (i + 1 >= areas.Count)
                        {
                            acerodir2 = numerodebarrasmaximo2;
                            cuantiar = acerodir2 * areas[i];
                            cuantiar = Math.Round(cuantiar * 100 / (diametro * diametro * Math.PI / 4), 2);
                            //MessageBox.Show("Maximum number of reinforcement reached for column " + (diametro).ToString() + "D for a quantity of " + cuantiar + "%.");
                            sectname = "X";
                            break;
                        }
                    }
                    else
                    {
                        cuantiar = acerodir2 * areas[i];
                        cuantiar = Math.Round(cuantiar * 100 / (diametro * diametro * Math.PI / 4), 2);
                        sectname = ("C" + diametro + "D_" + cuantiar + "%_" + acerodir2.ToString() + "_" + rebarsize[i]);
                        break;
                    }

                    i = i + 1;
                    numerodebarrasminimo2 = (int)Math.Ceiling((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmax + diam[i]));
                    numerodebarrasmaximo2 = (int)Math.Floor((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmin + diam[i]));
                }
                if (sectname == "X")
                {
                    sectname = ("C" + diametro + "D_" + cuantiar + "%_" + acerodir2.ToString() + "_" + rebarsize[i]);
                    crearseccion(sectname, 0, 0, acerodir2, rebarsize[i]);
                    ret = mySapModel.PropFrame.SetCircle(sectname, data.frame_Section.matprop, data.frame_Section.t3, -1, "Maximum number of reinforcement reached for column " + (diametro).ToString() + "D for a quantity of " + cuantiar + "%.","");

                    break;
                }
                if (cmin == 0.01)
                {
                    ret = mySapModel.PropFrame.ChangeName(name, sectname);
                }

                crearseccion(sectname, 0, 0, acerodir2, rebarsize[i]);
                cmin = cmin + cvar;
            }


        }

        public void ciclocuantiacolumnarectangular(double alto, double ancho, string name)
        {
            i = 0;
            barrasminmax(alto, ancho, diam[i], diam[i], data.column_Rebar.REstdiam, data.column_Rebar.Cover);
            cmin = 0.01;
            while (cmin <= cmax)
            {

                while (i < areas.Count)
                {
                    cuantia = alto * ancho * cmin - areas[i] * 4;

                    distribuiracerorectangular(alto, ancho, areas[i], areas[i]);
                    if (cuantia > (acerodir2 + acerodir3) * areas[i] * 2)
                    {
                        if (i + 1 < areas.Count)
                        {
                            cuantia = alto * ancho * cmin - areas[i + 1] * 4;
                            barrasminmax(alto, ancho, diam[i], diam[i + 1], data.column_Rebar.REstdiam, data.column_Rebar.Cover);
                            distribuiracerorectangular(alto, ancho, areas[i], areas[i + 1]);
                            if (cuantia <= (acerodir2 + acerodir3) * areas[i] * 2)
                            {
                                sectname = ("C" + alto + "X" + ancho + "_" + cuantiar + "%_4_" + rebarsize[i] + "_" + ((acerodir2 + acerodir3) * 2).ToString() + "_" + rebarsize[i]);
                                break;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("Maximum number of reinforcement reached for column " + (alto).ToString() + "X" + (ancho).ToString() + " for a quantity of " + cuantiar + "%.");
                            sectname = "X";
                            break;
                        }
                    }
                    else
                    {

                        sectname = ("C" + alto + "X" + ancho + "_" + cuantiar + "%_4_" + rebarsize[i] + "_" + ((acerodir2 + acerodir3) * 2).ToString() + "_" + rebarsize[i]);
                        break;
                    }
                    i = i + 1;
                    barrasminmax(alto, ancho, diam[i], diam[i], data.column_Rebar.REstdiam, data.column_Rebar.Cover);
                }
                if (sectname == "X")
                {
                    sectname = ("C" + alto + "X" + ancho + "_" + cuantiar + "%_4_" + rebarsize[i] + "_" + ((acerodir2 + acerodir3) * 2).ToString() + "_" + rebarsize[i]);
                    crearseccion(sectname, acerodir3 + 2, acerodir2 + 2, 0, rebarsize[i]);
                    ret = mySapModel.PropFrame.SetRectangle(sectname, data.frame_Section.matprop, data.frame_Section.t3, data.frame_Section.t2, -1, "Maximum number of reinforcement reached for column " + (alto).ToString() + "X" + (ancho).ToString() + " for a quantity of " + cuantiar + "%.","");

                    break;
                }
                if (cmin == 0.01)
                {
                    ret = mySapModel.PropFrame.ChangeName(name, sectname);
                }

                crearseccion(sectname, acerodir3 + 2, acerodir2 + 2, 0, rebarsize[i]);
                cmin = cmin + cvar;
            }


        }

        public void crearseccion(string sect, int blon3, int blon2, int blong, string barnamelong)
        {
            if (!sec.Contains(sect))
            {
                sec.Add(sect);

                if(blong == 0)
                {
                    ret = mySapModel.PropFrame.SetRectangle(sect, data.frame_Section.matprop, data.frame_Section.t3, data.frame_Section.t2);
                }
                else
                {
                    ret = mySapModel.PropFrame.SetCircle(sect, data.frame_Section.matprop, data.frame_Section.t3);
                }
                

                ret = mySapModel.PropFrame.SetRebarColumn(sect,
                    data.column_Rebar.MatPropLong,
                    data.column_Rebar.MatPropConfine,
                    data.column_Rebar.Pattern,
                    data.column_Rebar.ConfineType,
                    data.column_Rebar.Cover,
                    blong,
                    blon3,
                    blon2,
                    barnamelong,
                    data.column_Rebar.TieSize,
                    data.column_Rebar.TieSpacingLongit,
                    (int)Math.Floor((double)blon2 / 4) * 2 + 2,
                    (int)Math.Floor((double)blon3 / 4) * 2 + 2,
                    false);
            }








        }

        //public void reiniciodevariables()
        //{
        //    //cmin = double.Parse(Cmin.Text);
        //    //cmax = double.Parse(Cmax.Text);
        //    //cvar = double.Parse(Cvariacion.Text);

        //}


        public bool IsntNumeric(object Expression)

        {

            bool isNum;

            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return !isNum;

        }
        public void barrasminmax(double alto, double ancho, double dl, double dq, double dest, double rec)
        {
            numerodebarrasminimo2 = (int)Math.Ceiling((alto - 2 * rec - 2 * dq - 2 * dest - espmax) / (dl + espmax));
            numerodebarrasminimo3 = (int)Math.Ceiling((ancho - 2 * rec - 2 * dq - 2 * dest - espmax) / (dl + espmax));
            if (numerodebarrasminimo2 < 0)
                numerodebarrasminimo2 = 0;
            if (numerodebarrasminimo3 < 0)
                numerodebarrasminimo3 = 0;
            numerodebarrasmaximo2 = (int)Math.Floor((alto - 2 * rec - 2 * dq - 2 * dest - espmin) / (dl + espmin));
            numerodebarrasmaximo3 = (int)Math.Floor((ancho - 2 * rec - 2 * dq - 2 * dest - espmin) / (dl + espmin));
            if (numerodebarrasmaximo2 < 0)
                numerodebarrasmaximo2 = 0;
            if (numerodebarrasmaximo3 < 0)
                numerodebarrasmaximo3 = 0;
        }

        public void distribuiracerorectangular(double alto, double ancho, double bl, double bq)
        {
            if (alto * (fr - 1) / fr <= ancho & ancho <= alto * (fr + 1) / fr)
            {
                acerodir2 = (int)Math.Ceiling(cuantia / 4 / bl);
                acerodir3 = acerodir2;
            }
            else
            {
                acerodir2 = (int)Math.Ceiling(cuantia / 2 * alto * alto / (alto * alto + ancho * ancho) / bl);
                acerodir3 = (int)Math.Ceiling((cuantia / 2 - acerodir2 * bl) / bl);
            }
            if (acerodir2 < numerodebarrasminimo2)
            {
                acerodir2 = numerodebarrasminimo2;
                acerodir3 = (int)Math.Ceiling((cuantia / 2 - acerodir2 * bl) / bl);
            }
            if (acerodir3 < numerodebarrasminimo3)
            {
                acerodir3 = numerodebarrasminimo2;
                acerodir2 = (int)Math.Ceiling((cuantia / 2 - acerodir3 * bl) / bl);
                if (acerodir2 < numerodebarrasminimo2)
                {
                    acerodir2 = numerodebarrasminimo2;
                }
            }
            if (acerodir2 > numerodebarrasmaximo2)
            {
                acerodir2 = numerodebarrasmaximo2;
                acerodir3 = (int)Math.Ceiling((cuantia / 2 - acerodir2 * bl) / bl);
            }
            if (acerodir3 > numerodebarrasmaximo3)
            {
                acerodir3 = numerodebarrasmaximo3;
                acerodir2 = (int)Math.Ceiling((cuantia / 2 - acerodir3 * bl) / bl);
                if (acerodir2 > numerodebarrasmaximo2)
                {
                    acerodir2 = numerodebarrasmaximo2;
                }
            }
            cuantiar = 4 * bq + acerodir2 * bl * 2 + acerodir3 * bl * 2;
            cuantiar = Math.Round(cuantiar * 100 / (alto * ancho), 2);
        }

    }
}

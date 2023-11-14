using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ETABSv1;

namespace Columns_Sections
{
    public partial class Form1 : Form
    {
#region variables
        public double cmin;
        public double cmax;
        public double cvar;
        public double cuantia;
        public int acerodir2;
        public int acerodir3;
        public int numerodebarrasminimo2;
        public int numerodebarrasminimo3;
        public int numerodebarrasmaximo2;
        public int numerodebarrasmaximo3;
        public double cuantiar;
        public double fr = 6;                          //relacion ancho alto
        public double fce = 1;                         //factor de escala
        public double espmax = 20;
        public double espmin = 2.54;
        public List<double> diam = new List<double>();                           // diametros de barras
        public List<double> areas = new List<double>();                           // areas de barras
        public List<string> rebarsize = new List<string>();                 // nombre de las barras
        public List<string> sec = new List<string>();                       // secciones creadas
        public string sectname;                        // secciones nombres
        /*int i = 0;  */                            //contador

        protected cPlugin ParentPluginObject;
        protected cSapModel mySapModel;
        protected cPluginCallback ISapPlugin;
        protected Datos data;
        public int ret = 0;
        public eFramePropType typeoapy;

        #endregion

        public void setParentPluginObject(cPlugin inParentPluginObject)
        {
            ParentPluginObject = inParentPluginObject;

        }
        public void setSapModel(cSapModel inSapModel, cPluginCallback inISapPlugin, Datos Idata)
        {
            mySapModel = inSapModel;
            ISapPlugin = inISapPlugin;
            data = Idata;

        }
        public int typerebar(string seccion, int tipo, cSapModel mySapModel)
        {
            ret = mySapModel.PropFrame.GetTypeRebar //Lee nombre de la seccion y decide si se diseña como viga o columna
               (
                   seccion,
                   ref tipo
               );
            return tipo;
        }

        public void rebarlistfill()
        {
            ret = mySapModel.PropRebar.GetNameList(ref data.rebardata.NumberNames, ref data.rebardata.MyName);
            foreach (string name in data.rebardata.MyName)
            {
                double a = 0;
                double b = 0;
                ret = mySapModel.PropRebar.GetRebarProps(name, ref a, ref b);
                data.rebardata.Areas.Add(a);
                data.rebardata.Diameters.Add(b);
                Rebarlist.Items.Add(name);

            }

            ret = mySapModel.PropFrame.GetNameList(ref data.frame_Section.NumberNames, ref data.frame_Section.MyName);
        }

        public Form1()
        {
            InitializeComponent();
            
        }

        private void button_Click_Click(object sender, EventArgs e)
        {
            diam.Clear();
            areas.Clear();
            rebarsize.Clear();



            if (IsntNumeric(Cmini.Text))
            {
                Cmini.Text = "1";
            }
            if (IsntNumeric(Cmaxi.Text))
            {
                Cmaxi.Text = "6";
            }
            if (IsntNumeric(Cvariacion.Text))
            {
                Cvariacion.Text = "1";
            }

            cmin = double.Parse(Cmini.Text) / 100;
            cmax = double.Parse(Cmaxi.Text) / 100;
            cvar = double.Parse(Cvariacion.Text) / 100;
            for (int i = 0; i < data.rebardata.MyName.Length; i++)
            {
                if (Rebarlist.CheckedItems.Contains(data.rebardata.MyName[i]))
                {
                    diam.Add(data.rebardata.Diameters[i]);
                    areas.Add(data.rebardata.Areas[i]);
                    rebarsize.Add(data.rebardata.MyName[i]);
                }
            }

            espmax = espmax * fce;
            espmin = espmin * fce;
            foreach (string name in data.frame_Section.MyName)
            {
                //MessageBox.Show(name);

                
                //MessageBox.Show(data.column_Rebar.mytipe.ToString());
                // Obtiene el armado de las columnas rectangulares
                //MessageBox.Show(data.column_Rebar.mytipe.ToString());
                ret = mySapModel.PropFrame.GetTypeOAPI(name, ref typeoapy);
                if (typeoapy == eFramePropType.Circle || typeoapy == eFramePropType.Rectangular)
                {

                    data.column_Rebar.mytipe = typerebar(name, data.column_Rebar.mytipe, mySapModel);

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
                            for (int i = 0; i < diam.Count; i++)
                            {
                                ciclocuantiacolumnarectangular(data.frame_Section.t3, data.frame_Section.t2, name, i);
                            }

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
                            for (int i = 0; i < diam.Count; i++)
                            {
                                ciclocuantiacolumnacircular(data.frame_Section.t3, name, i);
                            }


                        }

                    }
                    else
                    {
                        sec.Add(name);
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

        public void ciclocuantiacolumnacircular(double diametro, string name, int i)
        {
            //i = 0;
            numerodebarrasminimo2 = (int)Math.Ceiling((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmax + diam[i]));
            numerodebarrasmaximo2 = (int)Math.Floor((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmin + diam[i]));
            cmin = double.Parse(Cmini.Text) / 100;
            while (cmin <= cmax)
            {

                //while (i < areas.Count)
                //{
                cuantia = diametro * diametro * cmin * Math.PI / 4;
                acerodir2 = (int)Math.Ceiling(cuantia / areas[i]);
                if (acerodir2 < numerodebarrasminimo2)
                    acerodir2 = numerodebarrasminimo2;
                if (acerodir2 > numerodebarrasmaximo2)
                {
                    //if (i + 1 >= areas.Count)
                    //{
                    acerodir2 = numerodebarrasmaximo2;
                    cuantiar = acerodir2 * areas[i];
                    cuantiar = Math.Round(cuantiar * 100 / (diametro * diametro * Math.PI / 4), 2);
                    //MessageBox.Show("Maximum number of reinforcement reached for column " + (diametro).ToString() + "D for a quantity of " + cuantiar + "%.");
                    sectname = "X";
                    //break;
                    //}
                }
                else
                {
                    cuantiar = acerodir2 * areas[i];
                    cuantiar = Math.Round(cuantiar * 100 / (diametro * diametro * Math.PI / 4), 2);
                    sectname = ("C" + diametro + "D_" + cuantiar + "%_" + acerodir2.ToString() + "_" + rebarsize[i]);
                    //break;
                }

                //i = i + 1;
                //numerodebarrasminimo2 = (int)Math.Ceiling((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmax + diam[i]));
                //numerodebarrasmaximo2 = (int)Math.Floor((diametro - 2 * data.column_Rebar.Cover) * Math.PI / (espmin + diam[i]));
                //}
                if (sectname == "X")
                {
                    sectname = ("C" + diametro + "D_" + cuantiar + "%_" + acerodir2.ToString() + "_" + rebarsize[i]);
                    crearseccion(sectname, 0, 0, acerodir2, rebarsize[i]);
                    ret = mySapModel.PropFrame.SetCircle(sectname, data.frame_Section.matprop, data.frame_Section.t3, -1, "Maximum number of reinforcement reached for column " + (diametro).ToString() + "D for a quantity of " + cuantiar + "%.", "");

                    break;
                }
                if (cmin == double.Parse(Cmini.Text) / 100)
                {
                    ret = mySapModel.PropFrame.ChangeName(name, sectname);
                }

                crearseccion(sectname, 0, 0, acerodir2, rebarsize[i]);
                cmin = cmin + cvar;
            }


        }

        public void ciclocuantiacolumnarectangular(double alto, double ancho, string name, int i)
        {
            //i = 0;
            barrasminmax(alto, ancho, diam[i], diam[i], data.column_Rebar.REstdiam, data.column_Rebar.Cover);
            cmin = double.Parse(Cmini.Text) / 100;
            while (cmin <= cmax)
            {

                //while (i < areas.Count)
                //{
                cuantia = alto * ancho * cmin - areas[i] * 4;

                distribuiracerorectangular(alto, ancho, areas[i], areas[i]);
                if (cuantia > (acerodir2 + acerodir3) * areas[i] * 2)
                {
                    //if (i + 1 < areas.Count)
                    //{
                    //    //cuantia = alto * ancho * cmin - areas[i + 1] * 4;
                    //    //barrasminmax(alto, ancho, diam[i], diam[i + 1], data.column_Rebar.REstdiam, data.column_Rebar.Cover);
                    //    //distribuiracerorectangular(alto, ancho, areas[i], areas[i + 1]);
                    //    //if (cuantia <= (acerodir2 + acerodir3) * areas[i] * 2)
                    //    //{
                    //    //    sectname = ("C" + alto + "X" + ancho + "_" + cuantiar + "%_4_" + rebarsize[i] + "_" + ((acerodir2 + acerodir3) * 2).ToString() + "_" + rebarsize[i]);
                    //    //    //break;
                    //    //}
                    //}
                    //else
                    //{
                    //MessageBox.Show("Maximum number of reinforcement reached for column " + (alto).ToString() + "X" + (ancho).ToString() + " for a quantity of " + cuantiar + "%.");
                    sectname = "X";
                    //break;
                    //}
                }
                else
                {

                    sectname = ("C" + alto + "X" + ancho + "_" + cuantiar + "%_4_" + rebarsize[i] + "_" + ((acerodir2 + acerodir3) * 2).ToString() + "_" + rebarsize[i]);
                    //break;
                }
                //i = i + 1;
                //barrasminmax(alto, ancho, diam[i], diam[i], data.column_Rebar.REstdiam, data.column_Rebar.Cover);
                //}
                if (sectname == "X")
                {
                    sectname = ("C" + alto + "X" + ancho + "_" + cuantiar + "%_4_" + rebarsize[i] + "_" + ((acerodir2 + acerodir3) * 2).ToString() + "_" + rebarsize[i]);
                    crearseccion(sectname, acerodir3 + 2, acerodir2 + 2, 0, rebarsize[i]);
                    ret = mySapModel.PropFrame.SetRectangle(sectname, data.frame_Section.matprop, data.frame_Section.t3, data.frame_Section.t2, -1, "Maximum number of reinforcement reached for column " + (alto).ToString() + "X" + (ancho).ToString() + " for a quantity of " + cuantiar + "%.", "");

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

                if (blong == 0)
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
        //    //cmin = double.Parse(Cmini.Text);
        //    //cmax = double.Parse(Cmaxi.Text);
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

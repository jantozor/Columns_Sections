using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ETABSv1;

namespace Columns_Sections
{
    public class cPlugin
    {

        public int ret = 0;
        //public ventana vent = new ventana();
        public Form1 wind = new Form1();


        public int Info(ref string Text)
        {
            try
            {
                Text = "This is the beta of the plugin for ETABS";
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

            return 0;
        }


        public void Main(ref cSapModel mySapModel, ref cPluginCallback ISapPlugin)
        {



            try
            {
                Datos data = new Datos();
                //vent.setParentPluginObject(this);
                //vent.setSapModel(mySapModel, ISapPlugin, data);

                wind.setParentPluginObject(this);
                wind.setSapModel(mySapModel, ISapPlugin, data);


                

                ret = mySapModel.SetPresentUnits(eUnits.Ton_cm_C);
                //Interfaz_Usuario Interfaz = new Interfaz_Usuario();
                //Data data = new Data();
                //DatosInnecesarios datosInecesarios = new DatosInnecesarios();

                //Lee las barras definidas en el ETABS
                //ret = mySapModel.PropRebar.GetNameList(ref data.rebardata.NumberNames, ref data.rebardata.MyName);
                //foreach (string name in data.rebardata.MyName)
                //{
                //    double a = 0;
                //    double b = 0;
                //    ret = mySapModel.PropRebar.GetRebarProps(name, ref a, ref b);
                //    data.rebardata.Areas.Add(a);
                //    data.rebardata.Diameters.Add(b);
                //    vent.Barramenor.Items.Add(name);
                //    vent.Barramayor.Items.Add(name);

                //}
                wind.rebarlistfill();
                //ret = mySapModel.PropFrame.GetNameList(ref data.frame_Section.NumberNames, ref data.frame_Section.MyName);
                wind.ShowDialog();
                //vent.ShowDialog();


            }

            catch (Exception ex)
            {
                //MessageBox.Show(("The following error terminated the Plugin:" + ("\r\n" + ex.Message)));

                try
                {
                    ISapPlugin.Finish(1);
                }
                catch (Exception ex1)
                {
                }

            }

        }



    }
}

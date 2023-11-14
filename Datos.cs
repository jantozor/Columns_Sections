using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns_Sections
{
    public class Datos
    {
        public Rebardata rebardata = new Rebardata();               // Datos sobre las barras definidas en el ETABS
        public Frame_Section frame_Section = new Frame_Section();   // Datos sobre la seccion transversal del elemento
        public Column_Rebar column_Rebar = new Column_Rebar();      // Datos sobre el armado de las columnas

    }

    public class Rebardata
    {
        public int NumberNames;
        public string[] MyName;
        public List<double> Areas = new List<double>();
        public List<double> Diameters = new List<double>();

        //    public string[] MyGUID;
    }

    public class Frame_Section
    {
        public int NumberNames;                     //Numero de elementos
        public string[] MyName;                     //Analisissect
        public double t3;                         //Alto
        public double t2;                         //Ancho
        public int color;
        public string note;
        public string guid;
        public string file;
        public string matprop;


    }

    public class Column_Rebar
    {
        public int mytipe;

        public string MatPropLong;                           
        public string MatPropConfine;                        
        public int Pattern;                                  
        public int ConfineType;                              
        public double Cover;                                 
        public int NumberCBars;                              
        public int NumberR3Bars;                             
        public int NumberR2Bars;                             
        public string RebarSize;                             
        public string TieSize;                               
        public double TieSpacingLongit;                      
        public int Number2DirTieBars;                        
        public int Number3DirTieBars;                        
        public bool ToBeDesigned;                            
                                                             
                            
        public double REstdiam;                              
        public double REstarea;          
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
                                         
    }



}

namespace ac.Models
{
    public class Materials
    {
        public string DEV_SN { get; set; }
        public string PrP { get; set; }
        public string PrintIDN { get; set; }
        public string MatName { get; set; }
        public string EIZ { get; set; }
        public double AmountOnWRHs { get; set; }
        public double RezervOnWRHs { get; set; }
        public double DeficitOnWRHs { get; set; }
        public double PlanPotreb { get; set; }
        public double CurrentPotreb { get; set; }
        public double Norma { get; set; }
        public string WRH { get; set; }
        public long rwc { get; set; }

        public string DOC { get; set; }

        public double GetFromWRH { get; set; }

        //public double AmOnWRHs
        //{
        //    get
        //    {
        //        if (AmountOnWRHs != null)
        //            return Math.Round(AmountOnWRHs, 2);
        //        else return 0;
        //    }
        //}

        //public double RezOnWRHs
        //{
        //    get
        //    {
        //        if (RezervOnWRHs != null)
        //            return Math.Round(RezervOnWRHs, 2);
        //        else return 0;
        //    }
        //}

        //public double DifOnWRHs
        //{
        //    get
        //    {
        //        if (DeficitOnWRHs != null)
        //            return Math.Round(DeficitOnWRHs, 2);
        //        else return 0;
        //    }
        //}

        //public double PlnPotreb
        //{
        //    get
        //    {
        //        if (PlanPotreb != null)
        //            return Math.Round(PlanPotreb, 2);
        //        else return 0;
        //    }
        //}

        //public double CurPotreb
        //{
        //    get
        //    {
        //        if (CurrentPotreb != null)
        //            return Math.Round(CurrentPotreb, 2);
        //        else return 0;
        //    }
        //}

        //public double Nrm
        //{
        //    get
        //    {
        //        if (Norma != null)
        //            return Math.Round(Norma, 2);
        //        else return 0;
        //    }
        //}

        public bool StatusBool { get; set; }
        public int Status { get; set; }
        public string EDName { get; set; }
    }
}

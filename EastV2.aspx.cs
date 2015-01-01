using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using EU.Iamia.Logging;
using EU.Iamia.NordPoolSpot.PriceProvider.Business;

namespace EU.Iamia.Elpriser
{
    public partial class EastV2 : Page
    {
        private static readonly ILog Logger = LogManager.GetLogger("EU.Iamia.Presentation");

        private List<PresentationEntity> BusinessData { get; set; }

        /// <summary>
        ///  Controls graph offsett.
        /// </summary>
        private decimal XpMinValueYsS { get; set; }

        /// <summary>
        /// Controls graph max value.
        /// </summary>
        private decimal XpMaxVaueYsS { get; set; }

        private BusinessContext zBusinessContext;
        private BusinessContext BusinessContext
        {
            get { return zBusinessContext ?? (zBusinessContext = new BusinessContext()); }
        }

        private static void CssModify(Style control, String addCss, params String[] removeCss)
        {
            foreach (var toRemove in removeCss)
            {
                var position = control.CssClass.IndexOf(toRemove, StringComparison.OrdinalIgnoreCase);
                if (position >= 0)
                {
                    control.CssClass = control.CssClass.Remove(position, toRemove.Length);
                }
            }

            if (!(String.IsNullOrWhiteSpace(addCss)) && control.CssClass.IndexOf(addCss, StringComparison.OrdinalIgnoreCase) < 0)
            {
                control.CssClass = String.Format("{0} {1}", control.CssClass, addCss);
            }

            control.CssClass = control.CssClass.Replace("  ", " ").Trim(); // remove double spaces.
        }

        private static void CssModify(WebControl control, String addCss, params String[] removeCss)
        {
            foreach (var toRemove in removeCss)
            {
                var position = control.CssClass.IndexOf(toRemove, StringComparison.OrdinalIgnoreCase);
                if (position >= 0)
                {
                    control.CssClass = control.CssClass.Remove(position, toRemove.Length);
                }
            }

            if (!(String.IsNullOrWhiteSpace(addCss)) && control.CssClass.IndexOf(addCss, StringComparison.OrdinalIgnoreCase) < 0)
            {
                control.CssClass = String.Format("{0} {1}", control.CssClass, addCss);
            }

            control.CssClass = control.CssClass.Replace("  ", " ").Trim(); // remove double spaces.
        }

        private void LoadBusinessData()
        {
            var requestType =
                XuYesterday.Checked
                    ? DayGroup.Yesterday
                    : XuToday.Checked
                          ? DayGroup.Today
                          : XuRolling.Checked
                                ? DayGroup.Rolling
                                : XuTomorrow.Checked
                                      ? DayGroup.Tomorrow
                                      : XuAll.Checked
                                            ? DayGroup.All
                                            : DayGroup.Rolling;

            BusinessContext.Read(requestType);

            var sortType =
                XuAsc.Checked
                    ? SortDirection.Ascending
                    : XuDesc.Checked
                          ? SortDirection.Descending
                          : SortDirection.Ascending;

            var sortColumn =
                XuSortColTime.Checked
                    ? ColumnName.Timeslot
                    : XuSortColCustomer.Checked
                          ? ColumnName.CustomerPrice
                          : XuSortColNordPool.Checked
                                ? ColumnName.NordPoolPrice
                                : ColumnName.CustomerPrice;

            BusinessContext.Sort(sortColumn, sortType);

            BusinessData = BusinessContext.TimeSlotInformation;
        }

        private class GridSource
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local

            /// <summary>
            /// Exact time when slot begins year, month, day, hour, 0, 0.
            /// </summary>
            public DateTime SlotBeginCurrent { get; set; }

            /// <summary>
            /// Exact time when next slot begins year, month, day, hour, 0, 0.
            /// </summary>
            public DateTime SlotBeginNext { get; set; }

            public decimal NordPoolSpotPrice { get; set; }
            public decimal CustomerPrice { get; set; }
            public decimal ReducedCustomerPrice { get; set; }
            public int GraphValue { get; set; }
            public Brush GraphColor { get; set; }
            public string Graph { get; set; }
            public int Rank { get; set; }

            // - calculation documentation --

            public String CpPriceListStep { get; set; }
            public DateTime CpValidFrom { get; set; }
            public decimal CpTransport { get; set; }
            public decimal CpPublicObligations { get; set; }
            public decimal CpElectricityTax { get; set; }
            public decimal CpTaxReduction { get; set; }

            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        private void XmSetupGrid()
        {
            var stop = BusinessData.Count;

            // Provide data for the Grid.
            var GridSourceList = new List<GridSource>(stop);
            {
                foreach (var entity in BusinessData)
                {
                    // ReSharper disable UseObjectOrCollectionInitializer

                    var gridSource = new GridSource();

                    gridSource.CpElectricityTax = entity.Pricelist.Elafgift;
                    gridSource.CpPriceListStep = entity.PriceListStep.ToString("G");
                    gridSource.CpPublicObligations = entity.Pricelist.OffentligeForpligtelser;
                    gridSource.CpTaxReduction = entity.Pricelist.Afgiftsreduktion;
                    switch (entity.PriceListStep)
                    {
                        case PriceListStep.T1:
                            gridSource.CpTransport = entity.Pricelist.TransportAfElTrin1;
                            break;
                        case PriceListStep.T2:
                            gridSource.CpTransport = entity.Pricelist.TransportAfElTrin2;
                            break;
                        case PriceListStep.T3:
                            gridSource.CpTransport = entity.Pricelist.TransportAfElTrin3;
                            break;
                    }
                    gridSource.CpValidFrom = entity.Pricelist.ValidFrom;

                    gridSource.CustomerPrice = entity.CustomerPrice; // incl VAT

                    // Multiply with pixel width of field.
                    var width = (int)(350 * (gridSource.CustomerPrice - XpMinValueYsS) / (XpMaxVaueYsS - XpMinValueYsS));
                    gridSource.GraphValue = Math.Max(1, width);

                    switch (entity.PriceGroup)
                    {
                        case PriceGroup.Lowest:
                            {
                                gridSource.GraphColor = Brushes.LightSkyBlue;
                                gridSource.Graph = ResolveUrl("~/Assets/Images/Blue.png");
                            }
                            break;

                        case PriceGroup.Low:
                            {
                                gridSource.GraphColor = Brushes.LightGreen;
                                gridSource.Graph = ResolveUrl("~/Assets/Images/Green.png");
                            }
                            break;

                        case PriceGroup.Mid:
                            {
                                gridSource.GraphColor = Brushes.Yellow;
                                gridSource.Graph = ResolveUrl("~/Assets/Images/Yellow.png");
                            }
                            break;

                        case PriceGroup.High:
                            {
                                gridSource.GraphColor = Brushes.Red;
                                gridSource.Graph = ResolveUrl("~/Assets/Images/Red.png");
                            }
                            break;
                    }

                    gridSource.NordPoolSpotPrice = entity.NordPoolPrice; // kr/MWh ex.VAT.

                    gridSource.ReducedCustomerPrice = entity.CustomerPriceElHeating;

                    gridSource.SlotBeginCurrent = entity.BeginOfTimeslot;
                    gridSource.SlotBeginNext = entity.BeginOfTimeslot.AddHours(1);

                    // Rank
                    gridSource.Rank = entity.PriceOrder;

                    // ReSharper restore UseObjectOrCollectionInitializer

                    GridSourceList.Add(gridSource);
                }
            }

            GridView1.AutoGenerateColumns = false;
            GridView1.DataSource = GridSourceList;
            GridView1.DataBind();

            //XuYesterday.Enabled = BusinessContext.HasYesterdayData;
            XuYesterday.Visible = BusinessContext.HasYesterdayData;

            //XuTomorrow.Enabled = BusinessContext.HasTomorrowData;
            XuTomorrow.Visible = BusinessContext.HasTomorrowData;

        }

        private static void SwitchVisibility(DataControlField target, DataControlRowType rowType, bool visible)
        {
            TableItemStyle styleControl = null;

            switch (rowType)
            {
                case DataControlRowType.Header:
                    styleControl = target.HeaderStyle;
                    break;

                case DataControlRowType.DataRow:
                    styleControl = target.ItemStyle;
                    break;

                case DataControlRowType.Footer:
                    styleControl = target.FooterStyle;
                    break;
            }

            if (styleControl == null) { return; }

            CssModify(
                       styleControl
                       , visible ? "Col_Visible" : "Col_Hidden"
                       , !visible ? "Col_Visible" : "Col_Hidden"
                       );

        }

        // see: http://stackoverflow.com/questions/785670/how-to-change-my-image-dataimageurlformatstring-value-inside-my-gridview-in-code

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs eventArgs)
        {
            var today = DateTime.Now.Date;

            var gridSource = eventArgs.Row.DataItem as GridSource;

            // ---Header and all rows ---
            if (eventArgs.Row.RowIndex < 1) // -1 (header), 0...n-1 (item rows)
            {
                foreach (DataControlFieldCell col in eventArgs.Row.Cells)
                {
                    // Note ContainingField is the definition not the individual rows.

                    var markercss = String.Format(" {0} ", col.ContainingField.ControlStyle.CssClass);

                    if (markercss.IndexOf(" MarkerCalculationOn ", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SwitchVisibility(col.ContainingField, eventArgs.Row.RowType, visible: XuShowCalculation.Checked);
                    }

                    if (markercss.IndexOf("MarkerCalculationOff ", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SwitchVisibility(col.ContainingField, eventArgs.Row.RowType,
                                         visible: !(XuShowCalculation.Checked));
                    }
                }
            }

            // --- Individual Rows ---
            if (eventArgs.Row.RowType == DataControlRowType.DataRow)
            {
                var showTomorrow = false;
                var showYesterday = false;

                if (gridSource != null)
                {
                    showTomorrow = gridSource.SlotBeginCurrent.Date > today;
                    showYesterday = gridSource.SlotBeginCurrent.Date < today;
                }

                foreach (DataControlFieldCell col in eventArgs.Row.Cells)
                {
                    var markercss = String.Format(" {0} ", col.ContainingField.ControlStyle.CssClass);

                    if (markercss.IndexOf(" MarkerDayOffsett ", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        CssModify(
                            col
                            , showTomorrow ? "RowTomorrow" : showYesterday ? "RowYesterday" : "RowToday"
                            , "RowTomorrow", "RowYesterday", "RowToday"
                            );
                    }

                    if (markercss.IndexOf(" MarkerGraph ", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (col.Controls.Count > 0)
                        {
                            var img = col.Controls[0] as System.Web.UI.WebControls.Image;
                            if (img != null && gridSource != null)
                            {
                                var width = gridSource.GraphValue;

                                img.ControlStyle.Width = width;
                                img.ControlStyle.Height = 20;
                            }
                        }
                    }
                }

            } // row

        }

        protected void Page_Load(object _Sender, EventArgs _E)
        {
            if (!(IsPostBack))
            {
                Logger.InfoFormat("Request from IP: {0}", Request.UserHostAddress);
            }

            XpMinValueYsS = 1.7m; // offesett graph
            XpMaxVaueYsS = 2.5m;  // limit graph 

            {
                LoadBusinessData();
                XmSetupGrid();
            }
        }

        protected void UxUpdate_Click(object sender, EventArgs e)
        { }

        protected void XuRolling_CheckedChanged(object sender, EventArgs e)
        { }

    }

}
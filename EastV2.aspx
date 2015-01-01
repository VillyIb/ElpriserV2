<%@ Page Title="Denmark East" AutoEventWireup="true" CodeBehind="EastV2.aspx.cs" Inherits="EU.Iamia.Elpriser.EastV2" Language="C#" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Assets/css/EastV2.aspx.css" rel="stylesheet" type="text/css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="DateSelection">
        <%-- time window --%>
        <asp:RadioButton ID="XuYesterday" runat="server" GroupName="XgWhen" OnCheckedChanged="UxUpdate_Click" Text="I går" AutoPostBack="True" />
        <asp:RadioButton ID="XuToday" runat="server" GroupName="XgWhen" OnCheckedChanged="UxUpdate_Click" Text="I dag" AutoPostBack="True" />
        <asp:RadioButton ID="XuRolling" runat="server" GroupName="XgWhen" Text="Rullende" OnCheckedChanged="UxUpdate_Click" AutoPostBack="True" Checked="True" />
        <asp:RadioButton ID="XuTomorrow" runat="server" GroupName="XgWhen" OnCheckedChanged="UxUpdate_Click" Text="I morgen" AutoPostBack="True" />
        <asp:RadioButton ID="XuAll" runat="server" GroupName="XgWhen" Text="Alle" OnCheckedChanged="UxUpdate_Click" AutoPostBack="True" />
        &nbsp;|&nbsp;
        <%-- sort order --%>
        <asp:RadioButton ID="XuAsc" runat="server" GroupName="XgSortOrder" OnCheckedChanged="UxUpdate_Click" Text="Stigende" AutoPostBack="True" Checked="True" />
        <asp:RadioButton ID="XuDesc" runat="server" GroupName="XgSortOrder" OnCheckedChanged="UxUpdate_Click" Text="Faldende" AutoPostBack="True" />
        &nbsp;|&nbsp;
        <%--cort column--%>
        <asp:RadioButton ID="XuSortColTime" runat="server" GroupName="XgSortColumn" OnCheckedChanged="UxUpdate_Click" Text="Tidspunkt" AutoPostBack="True" Checked="True" />
        <asp:RadioButton ID="XuSortColNordPool" runat="server" GroupName="XgSortColumn" OnCheckedChanged="UxUpdate_Click" Text="NordPool pris" AutoPostBack="True" />
        <asp:RadioButton ID="XuSortColCustomer" runat="server" GroupName="XgSortColumn" OnCheckedChanged="UxUpdate_Click" Text="Købspris" AutoPostBack="True" />
        &nbsp;|&nbsp;
        <asp:CheckBox ID="XuShowCalculation" runat="server" Text="Vis beregning" OnCheckedChanged="UxUpdate_Click" AutoPostBack="True" />
    </div>
    <div>
        <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound" Height="18">
            <Columns>
                <asp:BoundField HeaderText="Fra" DataField="SlotBeginCurrent" DataFormatString="{0:HH}" ControlStyle-CssClass="MarkerFrom MarkerDayOffsett" >
                    <HeaderStyle CssClass="Col_StartTime" ></HeaderStyle>
                    <ItemStyle CssClass="Col_StartTime Time"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Til" DataField="SlotBeginNext" DataFormatString="{0:HH}" ControlStyle-CssClass="MarkerTo MarkerDayOffsett">
                    <HeaderStyle CssClass="Col_EndTime"></HeaderStyle>
                    <ItemStyle CssClass="Col_EndTime Time" />
                </asp:BoundField>
                <asp:BoundField DataField="NordPoolSpotPrice" HeaderText="NordPoolSpot pris kr/MWh ex.moms" ReadOnly="True" DataFormatString="{0:0.00}" ControlStyle-CssClass="MarkerNordPoolSpotPrice MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_NordPoolSpot"></HeaderStyle>
                    <ItemStyle CssClass="Col_NordPoolSpot Number" />
                </asp:BoundField>
                <asp:BoundField DataField="CustomerPrice" HeaderText="Købspris kr/kWh" ReadOnly="True" DataFormatString="{0:0.000}" ControlStyle-CssClass="MarkerCustomerPrice MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_CustomerPrice"></HeaderStyle>
                    <ItemStyle CssClass="Col_CustomerPrice Number " />
                </asp:BoundField>
                <asp:BoundField DataField="CustomerPrice" HeaderText="Købspris kr/kWh" ReadOnly="True" DataFormatString="{0:0.00}" ControlStyle-CssClass="MarkerCustomerPrice MarkerCalculationOff">
                    <HeaderStyle CssClass="Col_CustomerPrice"></HeaderStyle>
                    <ItemStyle CssClass="Col_CustomerPrice Number " />
                </asp:BoundField>
                <asp:BoundField DataField="ReducedCustomerPrice" HeaderText="Elvarme pris kr/kWh" ReadOnly="True" DataFormatString="{0:0.00}" ControlStyle-CssClass="MarkerReducedCustomerPrice">
                    <HeaderStyle CssClass="Col_ReducedCustomerPrice"></HeaderStyle>
                    <ItemStyle CssClass="Col_ReducedCustomerPrice Number" />
                </asp:BoundField>
                <asp:ImageField DataImageUrlField="Graph" DataImageUrlFormatString="{0}" HeaderText="Købspris grafisk visning" ReadOnly="True" ControlStyle-CssClass="MarkerGraph MarkerCalculationOff">
                    <HeaderStyle CssClass="Col_GraphicalPrice"></HeaderStyle>
                    <ItemStyle CssClass="Col_GraphicalPrice Bar" />
                    <%--Note image is assigned class 'PriceImage" by code behind --%>
                </asp:ImageField>
                <asp:BoundField DataField="Rank" DataFormatString="{0}" HeaderText="Rank" ControlStyle-CssClass="MarkerRank MarkerCalculationOffX">
                    <HeaderStyle CssClass="Col_Rank"></HeaderStyle>
                    <ItemStyle  CssClass="Col_Rank Number" />
                </asp:BoundField>

                <%--Calculation Price List--%>                 
                <asp:BoundField DataField="CpPriceListStep" HeaderText="Pristrin" ReadOnly="True" ControlStyle-CssClass="MarkerCpPriceListStep MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_Calculation"></HeaderStyle>
                    <ItemStyle CssClass="Col_Calculation Text" />
                </asp:BoundField>

                <asp:BoundField DataField="CpValidFrom" HeaderText="Prisliste dato" ReadOnly="True" DataFormatString="{0:yyyy-MM-dd}" ControlStyle-CssClass="MarkerCpValidFrom MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_Calculation"></HeaderStyle>
                    <ItemStyle CssClass="Col_Calculation Text" />
                </asp:BoundField>

                <asp:BoundField DataField="CpTransport" HeaderText="Transport" ReadOnly="True" DataFormatString="{0:0.000}" ControlStyle-CssClass="MarkerCpTransport MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_Calculation"></HeaderStyle>
                    <ItemStyle CssClass="Col_Calculation Number" />
                </asp:BoundField>

                <asp:BoundField DataField="CpPublicObligations" HeaderText="Offentlige forpligtelser" ReadOnly="True" DataFormatString="{0:0.000}" ControlStyle-CssClass="MarkerCpPublicObligations MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_Calculation Col_Hidden"></HeaderStyle>
                    <ItemStyle CssClass="Col_Calculation Col_Hidden Number" />
                </asp:BoundField>

                <asp:BoundField DataField="CpElectricityTax" HeaderText="Elafgift" ReadOnly="True" DataFormatString="{0:0.000}" ControlStyle-CssClass="MarkerCpElectricityTax MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_Calculation"></HeaderStyle>
                    <ItemStyle CssClass="Col_Calculation Number" />
                </asp:BoundField>

                <asp:BoundField DataField="CpTaxReduction" HeaderText="Afgifts reduktion" ReadOnly="True" DataFormatString="{0:0.000}" ControlStyle-CssClass="MarkerCpTaxReduction MarkerCalculationOn">
                    <HeaderStyle CssClass="Col_Calculation"></HeaderStyle>
                    <ItemStyle CssClass="Col_Calculation Number" />
                </asp:BoundField>

<%--                <asp:BoundField DataField="CpXX" HeaderText="Calculation" ReadOnly="True" DataFormatString="{0:0.000}" ControlStyle-CssClass="MarkerXXX">
                    <HeaderStyle CssClass="Col_Calculation Col_Hidden"></HeaderStyle>
                    <ItemStyle CssClass="Col_Calculation Col_Hidden Text" />
                </asp:BoundField>--%>


            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>

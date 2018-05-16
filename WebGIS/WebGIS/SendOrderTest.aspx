<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendOrderTest.aspx.cs" Inherits="WebGIS.SendOrderTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="CID:"></asp:Label>

        <asp:TextBox ID="txtCID" runat="server" Width="68px">34674</asp:TextBox>

<%-- <asp:TextBox ID="TextBox2" runat="server" Width="68px">22139</asp:TextBox>--%>
        <asp:Label ID="TNO" runat="server" Text="TNO:"> </asp:Label>
 

        <asp:TextBox ID="txtTNO" runat="server" Width="81px">18345147165</asp:TextBox>

  <%-- <asp:TextBox ID="TextBox2" runat="server" Width="81px">7260</asp:TextBox>--%>
       

        <asp:TextBox ID="txtCarNo" runat="server" Width="81px">18345147165</asp:TextBox>
       <%-- <asp:TextBox ID="TextBox2" runat="server" Width="81px">黑A-F8538</asp:TextBox>--%>
 
        <asp:Button ID="btnSendPhoto" runat="server" Text="立即牌照" 
            onclick="btnSendPhoto_Click" Width="75px" />
        <asp:DropDownList ID="DropDownList1" runat="server">
            <asp:ListItem>0x00</asp:ListItem>
            <asp:ListItem>0x01</asp:ListItem>
            <asp:ListItem>0x02</asp:ListItem>
            <asp:ListItem>0x03</asp:ListItem>
            <asp:ListItem>0x04</asp:ListItem>
            <asp:ListItem>0x05</asp:ListItem>
            <asp:ListItem>0x06</asp:ListItem>
            <asp:ListItem>0x07</asp:ListItem>
            <asp:ListItem>0x08</asp:ListItem>
            <asp:ListItem>0x09</asp:ListItem>
            <asp:ListItem>0x10</asp:ListItem>
            <asp:ListItem>0x11</asp:ListItem>
            <asp:ListItem>0x12</asp:ListItem>
            <asp:ListItem>0x13</asp:ListItem>
            <asp:ListItem>0x14</asp:ListItem>
            <asp:ListItem>0x15</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="btnReadTerParam" runat="server" onclick="btnReadTerParam_Click" 
            Text="行驶记录仪参数读取" Width="139px" />
        <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
            Text="车辆点名" />
             <asp:TextBox ID="txtmaxSpeed" runat="server" Width="43px" >45</asp:TextBox>
        <asp:Button ID="btnOverSpeed" runat="server" onclick="btnOverSpeed_Click" 
            Text="车辆限速" />
        <asp:Button ID="btnIPSet" runat="server" onclick="btnIPSet_Click" Text="车辆呼转" /> <br />
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="获取指令结果" />

        <asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="获取指令结果(新)" />
        <asp:Button ID="Button3" runat="server" onclick="Button3_Click" Text="Button" />
        <br />
        <asp:TextBox ID="TextBox1" runat="server" Height="336px" Width="846px" 
            TextMode="MultiLine"></asp:TextBox> <br />
        <asp:Image ID="Image1" runat="server" Height="335px" Width="484px" 
            ImageUrl="~/UI/_styles/images/logincar.png" />
    </div>
    </form>
</body>
</html>

﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexSearch.aspx.cs" Inherits="HJD.AccessService.Host.IndexSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="text" runat="server" id="searchTxt" style="width:300px;border:1px solid #ccc;" /><asp:Button ID="searchBtn" runat="server" Text="Search" OnClick="searchBtn_Click" />
    </div>
    <br /><br />
    <div id="resultTitDiv" runat="server" style="color:#999;margin-bottom:10px;"></div>
    <div id="resultDiv" runat="server">
        
    </div>
    </form>
</body>
</html>


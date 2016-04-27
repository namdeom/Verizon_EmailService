<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailTestPage.aspx.cs" Inherits="EmailTestApp.EmailTestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
      <div><asp:Label ID="errorText" runat="server" ForeColor="red"></asp:Label></div>
    <div>
      <table>
          <tr>
               <td><asp:Label ID="Label1" runat="server" Text="From"></asp:Label></td>
               <td><asp:TextBox ID="txtFromEmail" runat="server"></asp:TextBox></td>
          </tr>
          <tr>
               <td><asp:Label ID="Label2" runat="server" Text="To"></asp:Label></td>
               <td><asp:TextBox ID="txtTo" runat="server"></asp:TextBox></td>
          </tr>
          <tr>
               <td><asp:Label ID="Label3" runat="server" Text="CC"></asp:Label></td>
               <td><asp:TextBox ID="txtCC" runat="server"></asp:TextBox></td>
          </tr>
           <tr>
               <td><asp:Label ID="Label4" runat="server" Text="Subject"></asp:Label></td>
               <td><asp:TextBox ID="txtSubject" runat="server"></asp:TextBox></td>
          </tr>
           <tr>
               <td><asp:Label ID="Label5" runat="server" Text="Body"></asp:Label></td>
               <td><asp:TextBox ID="txtBody" runat="server" Rows="5" Columns="60" TextMode="multiline"></asp:TextBox></td>
          </tr>
          <tr>
               <td><asp:Label ID="Label6" runat="server" Text="Attachments"></asp:Label></td>
               <td><asp:fileupload ID="fileAttachment" runat="server"></asp:fileupload></td>
          </tr>
          <tr>
               <td colspan="2"><asp:Button ID="btnSend" runat="server" Text="Send Email" CommandName="Sendmail" OnClick="btnSend_Click" /></td>
              <td colspan="2"><asp:Button ID="btnSend0" runat="server" Text="Send Email With InvalidCheck" CommandName="InvalidCheck" OnClick="btnSend0_Click"/></td>
          </tr>
      </table>
    </div>
       
        
       
    </form>
</body>
</html>


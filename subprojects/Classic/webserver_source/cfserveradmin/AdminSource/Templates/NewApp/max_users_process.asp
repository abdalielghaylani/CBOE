<%
'You can put whatever messages you want in this page or use a redirect
'For a redirection use javascript top.location.replace
'Redirection must be to a file outside of the application or the page you are
'redirecting to must have session.abandon on the bottom of the page.
'The only thing that must remain is session.Abandon


Response.Write "You have exceeded the number of concurrent users.<BR>"
Response.Write "Current Users = "  & Application("NumberCurrentUsers") & "<BR>"
Response.Write "Maximum Allowed = " & Application("MaxUsers")

'These should be the last 2 things on the page.
Session.Abandon
Response.End 'This can be removed if you are using a redirect
%>
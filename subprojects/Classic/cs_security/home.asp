<script LANGUAGE="javascript">

if (window.name != "")
{
	if (parent != "undefined")
	{
		parent.window.close();
	}
	if (self.opener != "undefined")
	{
		self.opener = this;
		self.close()
	}
}
else
{
	window.location = '/coemanager/forms/public/contentarea/home.aspx';
}
</script>

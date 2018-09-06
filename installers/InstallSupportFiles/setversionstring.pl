
use strict;
use Getopt::Long;
use File::Find;
use Cwd;

#
# Examples:
# .cs - [assembly: AssemblyDescription("")]
# .vb - <Assembly: AssemblyDescription("")>
#

my @AssemblyInfo_versionlines = ('assembly:\s*AssemblyDescription.*\(".*(\d*.\d*.\d*.\d*)"\)',
				 'assembly:\s*AssemblyFileVersion.*(".*")',
				 'assembly:\s*AssemblyVersion.*(".*")');
#
# Examples:
# .cs -  public const string AssemblyDescription = "E-Notebook 13.1.0.0";
# .vb -  Public Const AssemblyDescription As String = "E-Notebook 13.1.0.0"
#

my @AssemblyVersion_versionlines = ('AssemblyDescription.*".* (\d*.\d*.\d*.\d*)"',
				   'VersionShort.*(".*")',
				   'FileVersion.*(".*")',
				   'Version.*(".*")');
my $newversion;
my $newfileversion;
my $filename;

#
# get the command line options
#

if (!GetOptions("file=s" => \$filename,
		"version:s" => \$newversion,
		"fileversion:s" => \$newfileversion,
		"<>"     => \&usage)) {
  usage();
}

sub usage {
  print "SetVersionString -file=file name -version=version string -fileversion=version string\n";
  exit(1);
}

#
# there needs to be at least one version string
#
if (!$newversion && !$newfileversion) {
  print "No version strings supplied so no changes made to $filename\n";
  exit (0);
}

# create short version as the first 2 numbers in the version

$newversion =~ m/(\d*\.\d*)\./;
my $newshortversion = $1;

# if not fileversion was specified, use the version

if (!$newfileversion) {
  $newfileversion = $newversion;
}

# open, read in the entire file, close it, open for re-write

open FILE, "<$filename" or die "File: $filename not found\n";
my @lines = <FILE>;
close FILE;

print "Changing $File::Find::name\n";

# make the file writeable

chmod 0777, $filename;

# For Testing:
# open FILE, ">$filename.'new'";
open FILE, ">$filename";

# check the type of file (.cs or .vb) and select the correct version string

my @regexs;
if ($filename =~ /AssemblyVersion/i) {
  @regexs = @AssemblyVersion_versionlines;
}
else {
  @regexs = @AssemblyInfo_versionlines;
}

# find the respective info lines and set them to the new strings

foreach my $line (@lines) {
  foreach my $regex (@regexs) {
    #
    # replace whatever string is there with the new version
    #
    if ($line =~ /$regex/i) {
      my $replace = $1;
      if ($regex =~ /description/i) {
	if ($newversion) {
	  print "   $line";
	  $line =~ s/$replace/$newversion/;
	  print "   $line\n";
	}
      }
      elsif ($regex =~ /short/i) {
	if ($newshortversion) {
	  print "   $line";
	  $line =~ s/$replace/\"$newshortversion\"/;
	  print "   $line\n";
	}
      }
      elsif ($regex =~ /fileversion/i) {
	if ($newfileversion) {
	  print "   $line";
	  $line =~ s/$replace/\"$newfileversion\"/i;
	  print "   $line\n";
	}
      }
      else { # assembly version
	if ($newversion) {
	  print "   $line";
	  $line =~ s/$replace/\"$newversion\"/i;
	  print "   $line\n";
	}
      }
      last;
    }
  }
  print FILE $line;
}

close FILE;

      

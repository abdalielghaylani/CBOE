When a class is determined to no longer be in use:

(1) move the class to the 'Retired' folder, then
(2) comment out the entire contents of the class file
(3) Run through all available testing mechanisms
	* ensures that 3rd party code deosn't dynamically invoke that Class or its methods

After the subsequent product release, eliminate the commented-out classes from the project.

If at a later date they are found to be necessary, they can be obtained from Source Code Control.
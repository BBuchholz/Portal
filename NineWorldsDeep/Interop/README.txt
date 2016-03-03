These instructions are adapted from:
http://www.andrewt.com/blog/post/2013/06/15/Fun-with-MTP-in-C.aspx

I am only putting them here so they are in the repo in case that link
ever breaks. This was the method that got the library working for me :)

1) Add COM references for PortableDeviceApi and PortableDeviceTypes

2) Build project to generate interop assemblies

3) Copy generated Interop.PortableDeviceApiLib.dll from obj\Debug to
   another folder (this project uses Interop as suggested in the blog above)

4) Dissassemble the generated dll by running the following in the 
   developer command prompt for visual studio:

	ildasm Interop.PortableDeviceApiLib.dll /out:pdapi.il

5) Open pdapi.il in a text editor and make the following changes:

	a) Replace all instances of:
		
		GetDevices([in][out] string& marshal(lpwstr) pPnPDeviceIDs,

	   with:

		GetDevices([in][out] string[] marshal(lpwstr[]) pPnPDeviceIDs,

	b) Then for all instances of GetDeviceFriendlyName, GetDeviceDescription
	   and GetDeviceManufacturer replace:

		[in][out] uint16&

	   with:

		[in][out] uint16[] marshal([])

6) Rename original interop dll and reassemble the new one with:

	ilasm pdapi.il /dll /output=Interop.PortableDeviceApiLib.dll

7) Remove original reference to PortableDeviceApiLib and add
   a reference for the new one you just assembled

Torej lepo po vrsti...

1. Najprej gremo na prvo stran 
	- shranimo vse linke v List objektov:
		- delimo jih na različne datoteke:
			- Javascript datoteke
			- Slike (png, jpeg, jpg, tiff, bmp,...)
			- Ostal Resource
	- Pogledamo kateri linki so sledljivi in kateri ne,
	to označimo v liusti objektov kateri se rekruzivno pregledajo (linki)



1.
- pogledam stran
- shranim si vse linke ki so lokalni (začnejo se z / ali pač nimajo https ali https spredaj)

2.
Ko imam listo vseh linkov
shranim vse datoteke, to naredim na sledeci nacin:
	- pogledam ali v absolutni poti linka obstaja že istoimenski lokalni direktorij
	- če ne obstaja ga kreiram in naredim http request na link da shranim vsebino datoteke lokalno

3.
To naredim za vse strani

4. 
pregledam izjeme.



ZA DATOTEKE KOT SO JS, CSS, Jpeg, PNG in ICO

Rabm funkcijo ki vzame pravi a href url in:
1. Naredi direktorij če ne obstaja
2. Skopira notri Resource če ne obstajajo


Še boljše
Rabim funkcijo ki naredi direktorij in skopira notri vse tudi htmlje.
 


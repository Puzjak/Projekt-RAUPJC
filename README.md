# Projekt-RAUPJC

Link aplikacije: https://frizerskistudioelegant.azurewebsites.net/

Aplikacija nije do kraja završena. Ima osnovnu funkcionalnost, ali fali dizajn i još neke site stvari tako da se nadam da ćete barem ovaj back-end dio ocijeniti :)

Ova aplikacija služi kako bi se ljudi mogli preko interneta naručiti na šišanje. Korisnik za svaki dan i ovisno o duljini trajanja odabrane usluge dobije slobodne termine od kojih može izabrati jedan. Ti termini također ovise o broju radnika i radnom vremenu. Ako ima 2 radnika mogu biti izabrana dva termina u isto vrijeme. 
Aplikacija također ima kalendar koji prikazuje sve zauzete termine. Taj kalendar je ostvaren pomoću Fullcalendar plugina i trenutno ga se može samo gledati, ali kad otkrijem kako točno sve funkcionira dodati će se mogućnost da korisnik može kliknuti na svoj event na kalendaru il preko toga urediti opis ili ga izbrisati, a administratori i zaposlenici bi mogli to raditi sa svim terminima.
Kalendaru mogu pristupiti i ne registrirani korisnici.

Postoje 3 uloge: Customer, Employee, Administrator

1) Administrator
Prilikom prvog pokretanja aplikacije kreira se slijedeći admin račun</br>
username: admin@gmail.com</br>
password: Ovojesifrazaadmina1!</br>
Admin ima mogućnost da kreira i obriše role i usluge, dodijeljuje korisnicima role, briše korisnike i slično. 
Ako je korisnik u administrator roli na navbaru se pojavi dodatan Administrator gumb sa admin funkcionalnostima. 
Pri kreiranju se admin također stavi u Employee i Customer role.
Administrator ima pristup funkcijama Employee role, ali se ne treba nalaziti u toj roli. 

2) Employee
Rola koja predstavlja zaposlenika. Ako je korisnik u toj roli na navbaru se pojavi gumb Zaposlenik i preko toga gumba se dolazi do funkcija koje zaposlenici imaju. 

3)Customer
Rola samo za korisnike, omogućuje da korisnici mogu kreirati svoj termin i brisati ih. 

Korisnik neće moći obrisati termin ako je ostao 1 dan do izvršenja dok zaposlenici i administrator to mogu. 

Postoji mogućnost logina sa Facebookom, ali postoji "bug", a to je da se korisnici koji se prijave na ovaj način ne stave u Customer rolu, zbog čega je nad CustomerControllerom samo [Authorize] tag, ali još se problem stvara i kod dohvata svih klijenata jer se dohvaćaju svi korisnici koji su u Customer roli.

Otkriven bugovi
- eventi ostaju na kalendaru nakon što se izbrišu u bazi
- TermDatePartialView se ne poklapa sa nekim table headovima
- gumb "Opis" ne radi na nekim Viewovima, treba dodati skriptu
- uklanjanje uloge korisniku ne funkcionira

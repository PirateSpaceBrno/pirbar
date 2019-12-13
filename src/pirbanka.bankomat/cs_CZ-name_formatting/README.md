For English description, please see below.

Czech:

# Oslovení

Některé lidské jazyky používají pro oslovení speciální pád zvaný vokativ. Tento speciální pád implementuje jen velmi málo jazyků -- některé slovanské jazyky a také latina. Jiné jazyky vokativ nepoužívají.

V českém jazyce je vokativem pátý pád -- oslovujeme, voláme. Správné používání osolvení se u nás naučí již děti v předškolním věku, na prvním stupni základní školy pak při výuce českého jazyka pak používání pátého pádu vokalizujeme.

Zatímco v běžném jazyce je pro nás používání vokativu přirozené, v počítačových programech se jedná o oříšek. Je běžné, že známe nominativ (první pád) uživatele, nicméně pokud ho chceme oslovit, pak je to problém, jelikož vokativ (pátý pád) neznáme.

Stávající programy to řeší různě:

  * použijí nominativ a neřeší to (např. "Ahoj Petr")
  * pokud to situace umožňuje, změní větu tak, aby mohla používat nominativ (např. namísto "Ahoj " použijí "Přihlášený uživatel: ")
  * implementují algoritmus převodu nominativu na vokativ

Bohužel, poslední možnost je nejméně častá, jelikož dosavad neexistoval žádný volně přístupný algoritmus pro převod nominativu na vokativ. Tento stav tato knihovna řeší, nabízí takový algoritmus k volnému použití.

---

V ~~jednotlivých adresářích, nazvaných podle programovacích jazyků,~~ repozitáři najdete zdrojový kód funkce osloveni(), která pro předaný nominativ vrátí vokativ. Stačí přidat tuto funkci k vašemu programu a můžete začít používat vokativ tam, kde je to zapotřebí.

Funkce je k dispozici v ~~dnes běžně používaných jazycích PHP,~~ JavaScript *("Rozhodli jsme se přispívat pouze di Javascriptové knihovny, jelikož ostatní v našem projektu nevyužijeme. eJednání.cz)* ~~a Python. Navíc je k dispozici také v programovacím jazyku Micropython, který má jem minimální rozdíl oproti běžnému Pythonu.~~

~~*Upozornění:* Pro jazyk PHP funkce používá tvorbu tuples, která je k dispozici pro PHP verze 5.4 a novějších. Pokud používáte PHP starší, je zapotřebí si PHP verzi přegenerovat, viz níže.~~

---

Funkce neřeší, jestli se jedná o jméno mužské nebo ženské. Pro drtivou většinu případů to není problém, algoritmus to zvládá. Jediné jméno, u kterého je to problém (na které jsem přišel), je jméno Marin -- to lze použít jak pro muže, tak pro ženu, a pak se pátý pád liší. Tuto specialitu ale funkce neřeší -- použije se mužský rod ;-).

---

Zajímavosti:

  * Funkce respektuje používání velkých/malých písmen ve jménu pro možnosti vše malými písmeny (lowercase), vše velkými písmeny (uppercase) a první písmeno velkým a zbylá písmena malým (titlecase). Vokativ je vrácen ve stejném tvaru, v jakém byl předán nominativ
  * Při implementaci jsem používal správné vokativy dle Ústavu pro jazyk český. Zde jsem čerpal správné skloňování speciálních jmen, jako např. Artemis, Venus a podobně
  * Funkce používá správné skloňování jména Zeus, tedy pátý pád vrací Die
  * Funkce umí kromě křestních jmen také řadu jiných běžných slov, např. příjmení končící na -ová, jména některých zvířat (kuře, kůň a další) a podobně
  * Funkce umí skloňovat také řadu běžných anglických jmen, severských jmen, italských jmen
  * Funkce umí skloňovat jména z populárních děl, např. ze ságy Pán prstenů, ze seriálů Simpsonovi, Hra o trůny a další

---

English:

This algorithm solves converting nominative into vocative in Czech language. Function is available for javascript language.

---

## Licence

WTFPL License 2.0 applies

<code>           DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
                   Version 2, December 2004

Copyright (C) 2004 Sam Hocevar <sam@hocevar.net>

Everyone is permitted to copy and distribute verbatim or modified
copies of this license document, and changing it is allowed as long
as the name is changed.

           DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
  TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION

 0. You just DO WHAT THE FUCK YOU WANT TO.</code>

 ## Update by eJednání.cz
 - ponechána pouze Javascriptová knihovna, ostatní scripty odebrány
 - snaha o použití knihovny i pro příjmení
 - oprava pro jméno:
 Karel, 
 - oprava pro příjmení:
 Les, Pes, 
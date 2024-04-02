
/*
-- Movie Management System
21127367 - Đỗ Thế Nghĩa
21127384 - Dương Hạnh Nhi
21127461 - Lê Thành Trung
*/

CREATE DATABASE DB_MovieManagement
GO

USE DB_MovieManagement
GO

-- Tao cac bang du lieu
CREATE TABLE Account (
    Username NVARCHAR(30),
    Password NVARCHAR(100), -- Luu pass da hash
    DOB DATETIME,
    Gender NVARCHAR(8), -- Male || Female
    Fullname NVARCHAR(30),
    IsAdmin BIT, -- 1: Admin, 0: User
    AccountId INT not null IDENTITY(1,1) PRIMARY KEY
)

CREATE TABLE Bill (
    Total FLOAT,
    AccountId INT,
    BookingTime DATETIME,
    BillId INT not null IDENTITY(1,1) PRIMARY KEY
)

CREATE TABLE BillVoucher (
    BillId INT,
    VoucherId INT,
    AppliedTime DATETIME,
    PRIMARY KEY (BillId, VoucherId)
)

CREATE TABLE Voucher (
    VoucherCode NVARCHAR(30),
    DiscountAmount FLOAT,
    IsExpired BIT,
    IsPercentageDiscount BIT,
    RequirementAmount FLOAT,
    VoucherId INT not null IDENTITY(1,1) PRIMARY KEY
)

CREATE TABLE Ticket (
    TicketId INT not null IDENTITY(1,1) PRIMARY KEY,
    IsAvailable BIT,
    Row NVARCHAR(5),
    Col INT,
    Price FLOAT,
    BillId INT,
    ShowTimeId INT
)

CREATE TABLE ShowTime (
    ShowTimeId INT not null IDENTITY(1,1) PRIMARY KEY,
    MovieId INT,
    ShowDate DATETIME,
    MaxRow INT,
    MaxCol INT
)

CREATE TABLE Movie (
    Title NVARCHAR(30),
    Duration INT, -- Tinh theo phut
    PublishYear INT,
    IMDbScore FLOAT,
    AgeCertificateId INT,
    MovieId INT not null IDENTITY(1,1) PRIMARY KEY,
    IsGoldenHour BIT,
    IsBlockbuster BIT,
    PosterUrl NVARCHAR(100),
    TrailerUrl NVARCHAR(100),
    Description NVARCHAR(2000),
    GenreId INT
)

CREATE TABLE Person (
    Fullname NVARCHAR(30),
    AvatarUrl NVARCHAR(100),
    Biography NVARCHAR(2000),
    PersonId INT not null IDENTITY(1,1) PRIMARY KEY
)

CREATE TABLE Role (
    RoleName NVARCHAR(30),
    RoleId INT not null IDENTITY(1,1) PRIMARY KEY
)

CREATE TABLE Contributor (
    MovieId INT,
    PersonId INT,
    RoleId INT,
    PRIMARY KEY (MovieId, PersonId, RoleId)
)

CREATE TABLE Genre (
    GenreName NVARCHAR(30),
    GenreId INT not null IDENTITY(1,1) PRIMARY KEY
)

CREATE TABLE AgeCertificate (
    DisplayContent NVARCHAR(30),
    RequireAge INT,
    AgeCertificateId INT not null IDENTITY(1,1) PRIMARY KEY,
    BackgroundColor NVARCHAR(30),
    ForegroundColor NVARCHAR(30)
)


-- Rang buoc du lieu
GO
ALTER TABLE Bill ADD
    CONSTRAINT FK_Bill_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId)

GO
ALTER TABLE BillVoucher ADD
    CONSTRAINT FK_BillVoucher_Bill FOREIGN KEY (BillId) REFERENCES Bill(BillId),
    CONSTRAINT FK_BillVoucher_Voucher FOREIGN KEY (VoucherId) REFERENCES Voucher(VoucherId)

GO
ALTER TABLE Ticket ADD
    CONSTRAINT FK_Ticket_Bill FOREIGN KEY (BillId) REFERENCES Bill(BillId),
    CONSTRAINT FK_Ticket_ShowTime FOREIGN KEY (ShowTimeId) REFERENCES ShowTime(ShowTimeId)

GO
ALTER TABLE ShowTime ADD
    CONSTRAINT FK_ShowTime_Movie FOREIGN KEY (MovieId) REFERENCES Movie(MovieId)

GO
ALTER TABLE Movie ADD
    CONSTRAINT FK_Movie_Genre FOREIGN KEY (GenreId) REFERENCES Genre(GenreId),
    CONSTRAINT FK_AgeCertificate FOREIGN KEY (AgeCertificateId) REFERENCES AgeCertificate(AgeCertificateId)

GO
ALTER TABLE Contributor ADD
    CONSTRAINT FK_Contributor_Movie FOREIGN KEY (MovieId) REFERENCES Movie(MovieId),
    CONSTRAINT FK_Contributor_Person FOREIGN KEY (PersonId) REFERENCES Person(PersonId),
    CONSTRAINT FK_Contributor_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleId)


-- Nhap lieu cho cac bang
GO
-- /
INSERT INTO Voucher (voucherCode, discountAmount, isExpired, isPercentageDiscount,RequirementAmount) VALUES ('SUMMER25',    0.25, 0, 1, 1000);
INSERT INTO Voucher (voucherCode, discountAmount, isExpired, isPercentageDiscount,RequirementAmount) VALUES ('BACKTOSCHOOL', 0.3, 0, 1, 100000);
INSERT INTO Voucher (voucherCode, discountAmount, isExpired, isPercentageDiscount,RequirementAmount) VALUES ('U22',        40000, 0, 0, 0);
INSERT INTO Voucher (voucherCode, discountAmount, isExpired, isPercentageDiscount,RequirementAmount) VALUES ('SPRING20',     0.2, 0, 1, 100000);
INSERT INTO Voucher (voucherCode, discountAmount, isExpired, isPercentageDiscount,RequirementAmount) VALUES ('FALL40',       0.4, 1, 1, 1000);
INSERT INTO Voucher (voucherCode, discountAmount, isExpired, isPercentageDiscount,RequirementAmount) VALUES ('Birthday',0.2, 1, 1, 1000);


-- /
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('un1',      'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Male',   'John Doe', 0);
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('ad1',      'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Male',   'John Doe', 1);
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('trungle',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Male',   'Trung Le', 0);
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('nghiado',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Male',   'Nghia Do', 0);
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('yoshie',   'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Female', 'Nhi Duong', 0);
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('trungle1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Male',   'Le Trung', 1);
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('yoshie1',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Female', 'Duong Nhi', 1);
INSERT INTO Account (username, password, dob, gender, fullname, isAdmin) VALUES ('nghiado1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', '1990-05-15', 'Male',   'Do Nghia', 1);



-- #######################################

Insert into AgeCertificate(DisplayContent, RequireAge, BackgroundColor, ForegroundColor) values('R', 17, 'red', 'white');
Insert into AgeCertificate(DisplayContent, RequireAge, BackgroundColor, ForegroundColor) values('PG-13', 13, 'yellow', 'black');
Insert into AgeCertificate(DisplayContent, RequireAge, BackgroundColor, ForegroundColor) values('PG', 10, 'green', 'black');
Insert into AgeCertificate(DisplayContent, RequireAge, BackgroundColor, ForegroundColor) values('G', 0, 'blue', 'white');


-- /
Insert into Genre(GenreName) values('Action');
Insert into Genre(GenreName) values('Adventure');
Insert into Genre(GenreName) values('Comedy');
Insert into Genre(GenreName) values('Crime');
Insert into Genre(GenreName) values('Drama');
Insert into Genre(GenreName) values('Fantasy');
Insert into Genre(GenreName) values('Historical');
Insert into Genre(GenreName) values('Horror');
Insert into Genre(GenreName) values('Mystery');
Insert into Genre(GenreName) values('Romance');
Insert into Genre(GenreName) values('Science Fiction');
Insert into Genre(GenreName) values('Thriller');
Insert into Genre(GenreName) values('Western');



Insert into Person(Fullname, AvatarUrl, Biography) 
    values('Christopher Nolan', 'ms-appx:///Assets/avatars/christopher-nolan.jpg', 
           'Christopher Edward Nolan CBE is a British-American film director, producer, and screenwriter. He is one of the highest-grossing directors in history, and among the most successful and acclaimed filmmakers of the 21st century.'
    );
Insert into Person(Fullname, AvatarUrl, Biography) 
    values('Robert Downey', 'ms-appx:///Assets/avatars/robert-downey.jpg', 
           'Robert John Downey Jr. is an American actor and producer. His career has been characterized by critical and popular success in his youth, followed by a period of substance abuse and legal troubles, before a resurgence of commercial success in middle age.'
    );
Insert into Person(Fullname, AvatarUrl, Biography) 
    values('Leonardo Dicaprio', 'ms-appx:///Assets/avatars/leonardo-dicaprio.jpg', 
           'Few actors in the world have had a career quite as diverse as Leonardo DiCaprio.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Cillian Murphy', 'ms-appx:///Assets/avatars/cillian-murphy.jpg', 
           'Cillian Murphy is an Irish actor. He began his career performing as a rock musician. After turning down a record deal, he began his acting career in theatre, and in short and independent films in the late 1990s.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Timothee Chalamet', 'ms-appx:///Assets/avatars/timothee-chalamet.jpg', 
           'Timothee Hal Chalamet is an American actor. He began his acting career in short films, before appearing in the television drama series Homeland in 2012.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Joseph Gordon-Levitt', 'ms-appx:///Assets/avatars/joseph-gordon-levitt.jpg', 
           'Joseph Leonard Gordon-Levitt is an American actor, filmmaker, singer, and entrepreneur. As a child, Gordon-Levitt appeared in the films A River Runs Through It, Angels in the Outfield, Holy Matrimony and 10 Things I Hate About You, and as Tommy Solomon in the TV series 3rd Rock from the Sun.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Christian Bale', 'ms-appx:///Assets/avatars/christian-bale.jpg', 
           'Christian Charles Philip Bale is an English actor. Known for his versatility and intensive method acting, he is the recipient of many awards, including an Academy Award and two Golden Globe Awards, and was featured in the Time 100 list of 2011.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Heath Ledger', 'ms-appx:///Assets/avatars/heath-ledger.jpg', 
           'Heath Andrew Ledger was an Australian actor and music video director. After performing roles in several Australian television and film productions during the 1990s, Ledger left for the United States in 1998 to further develop his film career.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('John David Washington', 'ms-appx:///Assets/avatars/john-david-washington.jpg', 
           'John David Washington is an American actor, producer, and former American football running back. He played college football at Morehouse College and signed with the St. Louis Rams as an undrafted free agent in 2006.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Robert Pattinson', 'ms-appx:///Assets/avatars/robert-pattinson.jpg', 
           'Robert Douglas Thomas Pattinson is an English actor. Noted for his versatile roles in both big-budget and independent films, Pattinson has been ranked among the world''s highest-paid actors.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Mike Mitchell', 'ms-appx:///Assets/avatars/mike-mitchell.jpg', 
           'Mike Mitchell is an American film director, producer, actor, and former animator.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Jack Black', 'ms-appx:///Assets/avatars/jack-black.jpg', 
           'Thomas Jacob Black, known professionally as Jack Black, is an American actor, comedian, singer, and songwriter. His acting career has been extensive, starring primarily in comedy films.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Denis Villeneuve', 'ms-appx:///Assets/avatars/denis-villeneuve.jpg', 
           'Denis Villeneuve is a Canadian film director, writer, and producer. He is a four-time recipient of the Canadian Screen Award for Best Direction, for Maelström in 2001, Polytechnique in 2009, Incendies in 2010, and Enemy in 2013.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Tom Hiddleston', 'ms-appx:///Assets/avatars/tom-hiddleston.jpg', 
           'Thomas William Hiddleston is an English actor. He is the recipient of several accolades, including a Golden Globe Award and a Laurence Olivier Award, and has been nominated for two Primetime Emmy Awards.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Paul King', 'ms-appx:///Assets/avatars/paul-king.jpg', 
           'Paul King is an English film director, screenwriter, producer, and actor. He is best known for directing the comedy film Paddington and its sequel.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Aaron Horvath', 'ms-appx:///Assets/avatars/aaron-horvath.jpg', 
           'Aaron Horvath is an American animator, writer, and producer. He is best known for co-creating the animated television series Teen Titans Go! and directing the film adaptation Teen Titans Go! To the Movies.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Chris Pratt', 'ms-appx:///Assets/avatars/chris-pratt.jpg', 
           'Christopher Michael Pratt is an American actor, known for starring in both television and action films. He rose to prominence for his television roles, particularly as Andy Dwyer in the NBC sitcom Parks and Recreation.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Matthew McConaughey', 'ms-appx:///Assets/avatars/matthew-mcconaughey.jpg', 
           'Matthew David McConaughey is an American actor and producer. He first gained notice for his supporting performance in the coming-of-age comedy Dazed and Confused, which is considered by many to be the actor''s breakout role.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Anne Hathaway', 'ms-appx:///Assets/avatars/anne-hathaway.jpg', 
           'Anne Jacqueline Hathaway is an American actress. She is the recipient of various accolades, including an Academy Award, a Primetime Emmy Award, and a Golden Globe Award.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Fionn Whitehead', 'ms-appx:///Assets/avatars/fionn-whitehead.jpg', 
           'Fionn Whitehead is an English actor. He made his acting debut in the 2017 war film Dunkirk.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Henry Cavill', 'ms-appx:///Assets/avatars/henry-cavill.jpg', 
           'Henry William Dalgliesh Cavill is a British actor. He is known for his portrayal of the DC Comics character Superman in the DC Extended Universe, Geralt of Rivia in the Netflix fantasy series The Witcher, as well as Sherlock Holmes in the Netflix film Enola Holmes.'
    );
Insert into Person(Fullname, AvatarUrl, Biography)
    values('Anya Taylor-Joy', 'ms-appx:///Assets/avatars/anya-taylor-joy.jpg', 
           'Anya Josephine Marie Taylor-Joy is an American-born Argentine-British actress. She is the recipient of several accolades, including a Golden Globe Award, and has been nominated for a Critics'' Choice Movie Award, a Screen Actors Guild Award, and a BAFTA Rising Star Award.'
    );

Insert into Person (Fullname, AvatarUrl, Biography) values ('Emma Stone','ms-appx:///Assets/avatars/emma_stone.jpg','Emily Jean "Emma" Stone[a] (born November 6, 1988) is an American actress and producer. She is the recipient of various accolades, including two Academy Awards, two British Academy Film Awards, and two Golden Globe Awards.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Mark Ruffalo','ms-appx:///Assets/avatars/mark_ruffalo.jpg','Mark Alan Ruffalo is an American actor. He began acting in the early 1990s and first gained recognition for his work in Kenneth Lonergans play This Is Our Youth and drama film You Can Count on Me.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Yorgos Lanthimos','ms-appx:///Assets/avatars/yorgos_lanthimos.jpg','Yorgos Lanthimos was born in Athens, Greece. He studied directing for Film and Television at the Stavrakos Film School in Athens. He has directed a number of dance videos in collaboration with Greek choreographers, in addition to TV commercials, music videos, short films and theater plays.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Martin Scorsese','ms-appx:///Assets/avatars/martin_scorsese.jpg','Martin Charles Scorsese was born on November 17, 1942 in Queens, New York City, to Catherine Scorsese (née Cappa) and Charles Scorsese, who both worked in Manhattans garment district, and whose families both came from Palermo, Sicily. He was raised in the neighborhood of Little Italy, which later provided the inspiration for several of his films.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Jonah Hill','ms-appx:///Assets/avatars/jonah_hill.jpg','Jonah Hill was born and raised in Los Angeles, the son of Sharon Feldstein (née Chalkin), a fashion designer and costume stylist, and Richard Feldstein, a tour accountant for Guns N Roses. He is the brother of music manager Jordan Feldstein and actress Beanie Feldstein.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('David Fincher','ms-appx:///Assets/avatars/david_fincher.jpg','David Fincher was born in 1962 in Denver, Colorado, and was raised inMarin County, California. When he was 18 years old he went to work forJohn Korty at Korty Films in Mill Valley. Hesubsequently worked at ILM (Industrial Light and Magic) from 1981-1983.Fincher left ILM to direct TV commercials and music videos aftersigning with N. Lee Lacy in Hollywood.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Brad Pitt','ms-appx:///Assets/avatars/brad_pitt.jpg','William Bradley "Brad" Pitt was born on December 18, 1963 in Shawnee, Oklahoma and raised in Springfield, Missouri to Jane Etta Pitt (née Hillhouse), a school counselor & William Alvin "Bill" Pitt, a truck company manager. At Kickapoo High School, Pitt was involved in sports, debating, student government and school musicals. Pitt attended the University of Missouri, where he majored in journalism with a focus on advertising.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Edward Norton','ms-appx:///Assets/avatars/edward_norton.jpg','American actor, filmmaker and activist Edward Harrison Norton was born on August 18, 1969, in Boston, Massachusetts, and was raised in Columbia, Maryland.His mother, Lydia Robinson "Robin" (Rouse), was a foundation executive and teacher of English, and a daughter of famed real estate developer James Rouse, who developed Columbia, MD; she passed away of brain cancer on March 6, 1997.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Robert De Niro','ms-appx:///Assets/avatars/robert_de_niro.jpg','One of the greatest actors of all time, Robert De Niro was born on August 17, 1943 in Manhattan, New York City, to artists Virginia (Admiral) and Robert De Niro Sr. His paternal grandfather was of Italian descent, and his other ancestry is Irish, English, Dutch, German, and French. He was trained at the Stella Adler Conservatory and the American Workshop.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Nikolaj Arcel','ms-appx:///Assets/avatars/nikolaj_arcel','Nikolaj Arcel was born on 25 August 1972 in Copenhagen, Denmark. He is a writer and director, known for A Royal Affair (2012), Kongekabale (2004) and The Promised Land (2023).')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Mads Mikkelsen','ms-appx:///Assets/avatars/mads_mikkelsen.jpg','Mads Mikkelsens great successes parallel those achieved by the Danish film industry since the mid-1990s. He was born in Østerbro, Copenhagen, to Bente Christiansen, a nurse, and Henning Mikkelsen, a banker.Starting out as a low-life pusher/junkie in the 1996 success Pusher (1996), he slowly grew to become one of Denmarks biggest movie actors.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Amanda Collin','ms-appx:///Assets/avatars/amanda_collin.jpg','Amanda Collin was born on 4 March 1986 in Rungsted, Denmark. She is an actress, known for A Horrible Woman (2017), Department Q: A Conspiracy of Faith (2016) and Splitting Up Together (2016).')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Thomas Kail','ms-appx:///Assets/avatars/thomas_kail.jpg','Thomas Kail is known for Hamilton (2020), Fosse/Verdon (2019) and Grease Live! (2016). He has been married to Michelle Williams since March 2020. They have two children.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Lin-Manuel Miranda', 'ms-appx:///Assets/avatars/lin_manuel_miranda.jpg','Lin-Manuel Miranda wrote the first incarnation of "In the Heights" his sophomore year at Wesleyan University in Connecticut. Off-Broadway, "In The Heights" received nine Drama Desk nominations, including best music, best lyrics, and it won the award for outstanding ensemble performance; received the Lucille Lortel Award and Outer Critics Circle Award for best musical; received the Obie Award for outstanding music and lyrics; received a Theater World Award for outstanding debut Performance and the Clarence Derwent Award both for Mr. Miranda performance.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Phillipa Soo','ms-appx:///Assets/avatars/phillipa_soo.jpg','Phillipa Anne Soo is an American actress. Soo is best known for originating the role of Eliza Hamilton in the Broadway musical Hamilton, a performance which earned her a nomination for a 2016 Tony Award for Best Actress in a Leading Role in a Musical and a Grammy Award for Best Musical Theater Album in the same year.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Ava DuVernay','ms-appx:///Assets/avatars/ava_duvernay.jpg','A director, producer, writer, marketer and film distributor, Ava DuVernay made herfeature film debut with the documentary This is the Life (2008), ahistory on hip hop movement that flourished in Los Angeles in the 1990s. This was followed by series of television music documentarieswhich included My Mic Sounds Nice (2010) which aired on BET.DuVernays first narrative feature film, I Will Follow (2010), securedher the African-American Film Critics Association award for bestscreenplay.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Aunjanue Ellis-Taylor','ms-appx:///Assets/avatars/aunjanue_ellis_taylor.jpg','Aunjanue Ellis was born in San Francisco, California. She graduated from the Brown University, and later attended New York Universitys Tisch School of the Arts.During her career, Ellis performed on Off-Broadway theater, appeared in many film, and had roles on television. In film, she is best known for her roles in "Men of Honor" (2000), "Undercover Brother" (2002), "Ray" (2004), and "The Help" (2011).')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Jon Bernthal','ms-appx:///Assets/avatars/jon_bernthal.jpg','Jon Bernthal was born and raised in Washington D.C., the son of Joan (Marx) and Eric Bernthal, a lawyer. His grandfather was musician Murray Bernthal. Jon went to study at The Moscow Art Theatre School, in Moscow, Russia, where he also played professional baseball in the European professional baseball federation. While in Moscow, he was noticed by the director of Harvard Universitys Institute for Advanced Theatre Training at the American Repertory Theatre and was invited to obtain his M.F.A there.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Frank Darabont','ms-appx:///Assets/avatars/frank_darabont.jpg','Three-time Oscar nominee Frank Darabont was born in a refugee camp in1959 in Montbeliard, France, the son of Hungarian parents who had fledBudapest during the failed 1956 Hungarian revolution. Brought toAmerica as an infant, he settled with his family in Los Angeles andattended Hollywood High School.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Tim Robbins','ms-appx:///Assets/avatars/tim_robbins.jpg','Born in West Covina, California, but raised in New York City, TimRobbins is the son of formerThe Highwaymen singerGil Robbins and actressMary Robbins (née Bledsoe). Robbinsstudied drama at UCLA, where he graduated with honors in 1981. Thatsame year, he formed the Actors Gang theater group, an experimentalensemble that expressed radical political observations through theEuropean avant-garde form of theater.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Morgan Freeman','ms-appx:///Assets/avatars/morgan_freeman.jpg','With an authoritative voice and calm demeanor, this ever popular American actor has grown into one of the most respected figures inmodern US cinema. Morgan was born on June 1, 1937 in Memphis, Tennessee,to Mayme Edna (Revere), a teacher, and Morgan Porterfield Freeman, abarber.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Francis Ford Coppola','ms-appx:///Assets/avatars/francis_ford_coppola.jpg','Francis Ford Coppola was born in 1939 in Detroit, Michigan, but grew upin a New York suburb in a creative, supportive Italian-American family.His father, Carmine Coppola, was acomposer and musician. His mother,Italia Coppola (née Pennino), had been anactress.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Marlon Brando','ms-appx:///Assets/avatars/marlon_brando.jpg','Marlon Brando is widely considered the greatest movie actor of alltime, rivaled only by the more theatrically orientedLaurence Olivier in terms of esteem.Unlike Olivier, who preferred the stage to the screen, Brandoconcentrated his talents on movies after bidding the Broadway stageadieu in 1949, a decision for which he was severely criticized when hisstar began to dim in the 1960s and he was excoriated for squanderinghis talents.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Al Pacino','ms-appx:///Assets/avatars/al_pacino.jpg','Alfredo James "Al" Pacino established himself as a film actor during one of cinemas most vibrant decades, the 1970s, and has become an enduring and iconic figure in the world of American movies.He was born April 25, 1940 in Manhattan, New York City, to Italian-American parents, Rose (nee Gerardi) and Sal Pacino.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Sergio Leone', 'ms-appx:///Assets/avatars/sergio_leone.jpg',' Sergio Leone was virtually born into the cinema - he was the son of Roberto Roberti (A.K.A. Vincenzo Leone), one of Italy cinema pioneers, and actress Bice Valerian. Leone entered films in his late teens, working as an assistant director to both Italian directors and U.S. directors working in Italy (usually making Biblical and Roman epics, much in vogue at the time).')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Clint Eastwood','ms-appx:///Assets/avatars/clint_eastwood.jpg','Clint Eastwood was born May 31, 1930 in San Francisco, to Clinton Eastwood Sr., a bond salesman and later manufacturing executive for Georgia-Pacific Corporation, and Ruth Wood (née Margret Ruth Runner), a housewife turned IBM clerk. He grew up in nearby Piedmont. At school Clint took interest in music and mechanics, but was an otherwise bored student; this resulted in being held back a grade. In 1949, the year he is said to have graduated high school, his parents and younger sister Jeanne moved to Seattle.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Eli Wallach','ms-appx:///Assets/avatars/eli_wallach.jpg','One of Hollywoods finest character / "Method" actors, Eli Wallach was in demand for over 60 years (first film/TV role was 1949) on stageand screen, and has worked alongside the worlds biggest stars, includingClark Gable,Clint Eastwood,Steve McQueen,Marilyn Monroe,Yul Brynner,Peter O Toole, and Al Pacino')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Peter Jackson','ms-appx:///Assets/avatars/peter_jackson.jpg','Sir Peter Jackson made history with The Lord of the Rings trilogy, becoming the first person to direct three major feature films simultaneously. The Fellowship of the Ring, The Two Towers and The Return of the King were nominated for and collected a slew of awards from around the globe, with The Return of the King receiving his most impressive collection of awards.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Elijah Wood','ms-appx:///Assets/avatars/elijah_wood.jpg','Elijah Wood is an American actor best known for portraying FrodoBaggins in Peter Jacksons blockbuster Lord of the Rings film trilogy. In addition to reprisingthe role in The Hobbit series, Wood also played Ryanin the FX television comedy Wilfred (2011) and voiced Beck in the Disney XD animated television seriesTron: Uprising (2012).')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Ian McKellen','ms-appx:///Assets/avatars/ian_mckellen.jpg','Widely regarded as one of greatest stage and screen actors both in his native Great Britain and internationally, twice nominated for the Oscar and recipient of every major theatrical award in the UK and US, Ian Murray McKellen was born on May 25, 1939 in Burnley, Lancashire, England, to Margery Lois (Sutcliffe) and Denis Murray McKellen, a civil engineer and lay preacher.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Orlando Bloom','ms-appx:///Assets/avatars/orlando_bloom.jpg','Orlando Jonathan Blanchard Copeland Bloom was born on January 13, 1977 in Canterbury, Kent, England. His mother, Sonia Constance Josephine Bloom (née Copeland),was born in Kolkata, India, to an English family then-resident there.The man he first knew as his father, Harry Bloom, was a legendarypolitical activist who fought for civil rights in South Africa.')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Irvin Kershner','ms-appx:///Assets/avatars/irvin_kershner.jpg','Irvin Kershner was born on April 29, 1923 in Philadelphia,Pennsylvania. A graduate of the University of Southern California filmschool, Kershner began his career in 1950, producing documentaries forthe United States Information Service in the Middle East. He laterturned to television, directing and photographing a series ofdocumentaries called "Confidential File". Kershner was one of thedirectors given his first break by producerRoger Corman, for whom he shotStakeout on Dope Street (1958).')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Mark Hamill', 'ms-appx:///Assets/avatars/mark_hamill.jpg','Mark Hamill is best known for his portrayal of Luke Skywalker in the original Star Wars trilogy - Star Wars: Episode IV - A New Hope (1977), Star Wars: Episode V - The Empire Strikes Back (1980), and Star Wars: Episode VI - Return of the Jedi (1983) - a role he reprised in Star Wars: Episode VII - The Force Awakens (2015), Star Wars: Episode VIII - The Last Jedi (2017) and Star Wars: Episode IX - The Rise of Skywalker (2019).')
Insert into Person (Fullname, AvatarUrl, Biography) values ('Harrison Ford', 'ms-appx:///Assets/avatars/harrison_ford.jpg','Harrison Ford was born on July 13, 1942 in Chicago, Illinois, to Dorothy (Nidelman), a radio actress, and Christopher Ford (born John William Ford), an actor turned advertising executive. His father was of Irish and German ancestry, while his maternal grandparents were Jewish emigrants from Minsk, Belarus. Harrison was a lackluster student at Maine Township High School East in Park Ridge Illinois (no athletic star, never above a C average).')


INSERT INTO Role(RoleName) VALUES('Director');
INSERT INTO Role(RoleName) VALUES('Actor');


INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Oppenheimer',     180,        2023,       8.3,                1,            0,             1,
    'ms-appx:///Assets/thumbnails/oppenheimer.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'A biographical drama film about the life of J. Robert Oppenheimer, the American theoretical physicist who is often called the "father of the atomic bomb".',
    7);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Kung Fu Panda 4', 94,        2024,       6.5,                3,            1,             1,
    'ms-appx:///Assets/thumbnails/kungfu-panda-4.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'After Po is tapped to become the Spiritual Leader of the Valley of Peace, he needs to find and train a new Dragon Warrior, while a wicked sorceress plans to re-summon all the master villains whom Po has vanquished to the spirit realm.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Dune: Part Two',  166,        2024,       8.5,                2,            1,             1,
    'ms-appx:///Assets/thumbnails/dune-2.png',
    'ms-appx:///Assets/trailer.mp4',
    'Paul Atreides unites with Chani and the Fremen while seeking revenge against the conspirators who destroyed his family.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Super Mario Bros.', 92,      2023,       7.0,                3,            1,             1,
    'ms-appx:///Assets/thumbnails/mario.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'A plumber named Mario travels through an underground labyrinth with his brother Luigi, trying to save a captured princess.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId) 
    VALUES('Inception',       148,        2010,       8.8,                2,            1,             1, 
    'ms-appx:///Assets/thumbnails/inception.jpg',
    'ms-appx:///Assets/trailer.mp4', 
    'A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.', 
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId) 
    VALUES('The Dark Knight', 152,        2008,       9.0,                1,            0,             1, 
    'ms-appx:///Assets/thumbnails/the-dark-knight.jpg',
    'ms-appx:///Assets/trailer.mp4', 
    'When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.', 
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Tenet',           150,        2020,       7.3,                1,            1,             0,
    'ms-appx:///Assets/thumbnails/tenet.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'Armed with only one word, Tenet, and fighting for the survival of the entire world, a Protagonist journeys through a twilight world of international espionage on a mission that will unfold in something beyond real time.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Kung Fu Panda',  92,         2008,       7.6,                3,            1,             0,
    'ms-appx:///Assets/thumbnails/kungfu-panda-1.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'The Dragon Warrior has to clash against the savage Tai Lung as China''s fate hangs in the balance. However, the Dragon Warrior mantle is supposedly mistaken to be bestowed upon an obese panda who is a novice in martial arts.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Kung Fu Panda 2', 90,         2011,       7.3,                3,            1,             0,
    'ms-appx:///Assets/thumbnails/kungfu-panda-2.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'Po and his friends fight to stop a peacock villain from conquering China with a deadly new weapon, but first the Dragon Warrior must come to terms with his past.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Kung Fu Panda 3', 95,         2016,       7.1,                3,            1,             0,
    'ms-appx:///Assets/thumbnails/kungfu-panda-3.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'Continuing his "legendary adventures of awesomeness", Po must face two hugely epic, but different threats: one supernatural and the other a little closer to home.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Loki',            150,        2021,       8.5,                3,            1,             0,
    'ms-appx:///Assets/thumbnails/loki-1.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'The mercurial villain Loki resumes his role as the God of Mischief in a new series that takes place after the events of “Avengers: Endgame.”',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Loki 2',          140,         2023,       9.9,                3,           1,             1,
    'ms-appx:///Assets/thumbnails/loki-2.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'The mercurial villain Loki resumes his role as the God of Mischief in a new series that takes place after the events of “Avengers: Endgame.”',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Wonka',           120,        2023,       7.1,                3,            1,             0,
    'ms-appx:///Assets/thumbnails/wonka.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'The story of how a green-skinned, orange-haired, and purple-suited Willy Wonka came to own and operate his chocolate factory.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('The Prestige',   130,        2006,       8.5,                1,           1,             0,
    'ms-appx:///Assets/thumbnails/the-prestige.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'After a tragic accident, two stage magicians engage in a battle to create the ultimate illusion while sacrificing everything they have to outwit each other.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Interstellar',    169,        2014,       8.6,                1,            1,             0,
    'ms-appx:///Assets/thumbnails/interstellar.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'A team of explorers travel through a wormhole in space in an attempt to ensure humanity''s survival.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Dunkirk',         106,        2017,       7.9,                2,            0,             1,
    'ms-appx:///Assets/thumbnails/dunkirk.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'Allied soldiers from Belgium, the British Empire, and France are surrounded by the German Army and evacuated during a fierce battle in World War II.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Man of Steel',    143,        2013,       7.0,                1,            1,             1,
    'ms-appx:///Assets/thumbnails/man-of-steel.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'An alien child is evacuated from his dying world and sent to Earth to live among humans. His peace is threatened, when survivors of his home planet invade Earth.',
    11);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Memento',         113,        2000,       8.4,                1,            1,             0,
    'ms-appx:///Assets/thumbnails/memento.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'A man with short-term memory loss attempts to track down his wifes murderer.',
    11);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Poor Things',         141,        2023,       8.0,                1,            1,             1,
    'ms-appx:///Assets/thumbnails/poorthings.png',
    'ms-appx:///Assets/trailer.mp4',
    'The incredible tale about the fantastical evolution of Bella Baxter, a young woman brought back to life by the brilliant and unorthodox scientist Dr. Godwin Baxter.',
    10);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('The Wolf of Wall Street',         180,        2013,       8.2,                1,            0,             0,
    'ms-appx:///Assets/thumbnails/wolfofwallstreet.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'Based on the true story of Jordan Belfort, from his rise to a wealthy stock-broker living the high life to his fall involving crime, corruption and the federal government.',
    3);
INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Fight Club',         139,        1999,       8.8,                1,            0,             0,
    'ms-appx:///Assets/thumbnails/fightclub.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.',
    5);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Killers of the Flower Moon',         206,        2023,       7.6,                1,            1,             1,
    'ms-appx:///Assets/thumbnails/killers_of_the_flower_moon.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'When oil is discovered in 1920s Oklahoma under Osage Nation land, the Osage people are murdered one by one - until the FBI steps in to unravel the mystery.',
    4);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('The Promised Land',         127,        2023,       7.7,                1,            0,            1,
    'ms-appx:///Assets/thumbnails/the_promise_land.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'The story of Ludvig Kahlen who pursued his lifelong dream: To make the heath bring him wealth and honor.',
    7);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Hamilton',         160,        2020,       8.3,                2,            1,             0,
    'ms-appx:///Assets/thumbnails/hamilton.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'The real life of one of Americas foremost founding fathers and first Secretary of the Treasury, Alexander Hamilton. Captured live on Broadway from the Richard Rodgers Theater with the original Broadway cast.',
    5);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Origin',         141,        2023,       7.2,                2,            0,             0,
    'ms-appx:///Assets/thumbnails/origin.jpeg',
    'ms-appx:///Assets/trailer.mp4',
    'The unspoken system that has shaped America and chronicles how lives today are defined by a hierarchy of human divisions.',
    5);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('The Shawshank Redemption',         142,        1994,       9.3,                1,            0,             0,
    'ms-appx:///Assets/thumbnails/the_shawshank_redemption.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'Over the course of several years, two convicts form a friendship, seeking consolation and, eventually, redemption through basic compassion.',
    5);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('The Godfather',         175,        1972,       9.2,                1,            0,             0,
    'ms-appx:///Assets/thumbnails/the_godfather.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.',
    4);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('The Lord of the Rings',         178,        2001,       8.9,                1,            1,             0,
    'ms-appx:///Assets/thumbnails/the_lord_of_the_rings.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'A meek Hobbit from the Shire and eight companions set out on a journey to destroy the powerful One Ring and save Middle-earth from the Dark Lord Sauron.',
    1);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('The Good, the Bad and the Ugly',         178,        1966,       8.8,                4,            0,             0,
    'ms-appx:///Assets/thumbnails/the_good_the_bad_and_the_ugly.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'A bounty hunting scam joins two men in an uneasy alliance against a third in a race to find a fortune in gold buried in a remote cemetery..',
    2);

INSERT INTO Movie(Title, Duration, PublishYear, IMDbScore, AgeCertificateId, IsGoldenHour, IsBlockbuster, PosterUrl, TrailerUrl, Description, GenreId)
    VALUES('Star Wars: Episode V',         124,        1980,       8.7,                3,            1,             1,
    'ms-appx:///Assets/thumbnails/star_wars_episode_v.jpg',
    'ms-appx:///Assets/trailer.mp4',
    'After the Rebels are overpowered by the Empire, Luke Skywalker begins his Jedi training with Yoda, while his friends are pursued across the galaxy by Darth Vader and bounty hunter Boba Fett.',
    2);

-- Oppenheimer
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (1, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (1, 2, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (1, 4, 2);
-- Kung Fu Panda 4
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (2, 11, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (2, 12, 2);
-- Dune: Part Two
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (3, 13, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (3, 5, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (3, 22, 2);
-- Super Mario Bros.
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (4, 16, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (4, 17, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (4, 22, 2);
-- Inception
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (5, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (5, 3, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (5, 6, 2);
-- The Dark Knight
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (6, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (6, 7, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (6, 8, 2);
-- Tenet
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (7, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (7, 9, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (7, 10, 2);

-- Kung Fu Panda 1 2 3
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (8, 11, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (8, 12, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (9, 11, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (9, 12, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (10, 11, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (10, 12, 2);
-- Loki 1 2
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (11, 13, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (11, 14, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (12, 13, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (12, 14, 2);
-- Wonka
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (13, 15, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (13, 5, 2);
-- The Prestige
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (14, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (14, 7, 2);
-- Interstellar
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (15, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (15, 18, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (15, 19, 2);
-- Dunkirk
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (16, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (16, 20, 2);
-- Man of Steel
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (17, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (17, 21, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (17, 7, 2);
-- Memento
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (18, 1, 1);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (18, 18, 2);
INSERT INTO Contributor(MovieId, PersonId, RoleId) VALUES (18, 13, 2);

Insert into Contributor (MovieId,PersonId,RoleId) values (19,23,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (19,24,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (19,25,1)

Insert into Contributor (MovieId,PersonId,RoleId) values (20,26,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (20,27,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (20,3,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (21,28,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (21,29,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (21,30,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (22,31,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (22,3,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (22,26,1)

Insert into Contributor (MovieId,PersonId,RoleId) values (23,32,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (23,33,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (23,34,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (24,35,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (24,36,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (24,37,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (25,38,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (25,39,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (25,40,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (26,41,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (26,42,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (26,43,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (27,44,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (27,45,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (27,46,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (28,47,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (28,48,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (28,49,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (29,50,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (29,51,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (29,52,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (29,53,2)

Insert into Contributor (MovieId,PersonId,RoleId) values (30,54,1)
Insert into Contributor (MovieId,PersonId,RoleId) values (30,55,2)
Insert into Contributor (MovieId,PersonId,RoleId) values (30,56,2)


Insert into Bill (Total, AccountId, BookingTime) values (100000,   1,'2024-02-12');
Insert into Bill (Total, AccountId, BookingTime) values (100000,   1,'2024-03-13');
Insert into Bill (Total, AccountId, BookingTime) values (300000,   1,'2024-03-19');
Insert into Bill (Total, AccountId, BookingTime) values (100000,   3,'2024-03-01');
Insert into Bill (Total, AccountId, BookingTime) values (400000,   3,'2024-03-10');
Insert into Bill (Total, AccountId, BookingTime) values (600000,   3,'2024-03-18');
Insert into Bill (Total, AccountId, BookingTime) values (1000000,  4,'2024-02-28');
Insert into Bill (Total, AccountId, BookingTime) values (1500000, 3,'2024-03-05');
Insert into Bill (Total, AccountId, BookingTime) values (11000000, 5,'2024-03-24');


INSERT INTO ShowTime(MovieId, ShowDate, MaxRow, MaxCol) VALUES (1, '2024-02-12', 10, 10);
INSERT INTO ShowTime(MovieId, ShowDate, MaxRow, MaxCol) VALUES (2, '2024-03-13', 10, 10);
INSERT INTO ShowTime(MovieId, ShowDate, MaxRow, MaxCol) VALUES (3, '2024-04-19', 10, 10);
INSERT INTO ShowTime(MovieId, ShowDate, MaxRow, MaxCol) VALUES (4, '2024-02-01', 10, 10);
INSERT INTO ShowTime(MovieId, ShowDate, MaxRow, MaxCol) VALUES (5, '2023-10-18', 10, 10);

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-24 07:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-24 08:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-24 09:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-24 17:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-24 18:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-24 20:30',27,27)  

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-24 07:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-24 08:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-24 09:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-23 17:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-23 18:30',27,27)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-23 20:30',27,27)


Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (10,'2024-03-23 07:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (10,'2024-03-23 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (10,'2024-03-23 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (10,'2024-03-23 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (10,'2024-03-23 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (10,'2024-03-23 20:30',30,30)  

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (11,'2024-03-23 07:30',30,30)
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (11,'2024-03-23 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (11,'2024-03-23 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (11,'2024-03-23 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (11,'2024-03-23 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (11,'2024-03-23 20:30',30,30)

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-25 07:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-25 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-26 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-26 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-26 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-26 20:30',30,30)  

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-27 07:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-27 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-27 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-27 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-27 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (1,'2024-03-27 20:30',30,30)

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-25 07:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-25 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-26 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-26 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-26 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-26 20:30',30,30)  

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-27 07:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-27 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-27 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-27 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-27 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (2,'2024-03-27 20:30',30,30)

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-25 07:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-25 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-26 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-26 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-26 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-26 20:30',30,30)  

Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-27 07:30',30,30) 
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-27 08:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-27 09:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-27 17:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-27 18:30',30,30)  
Insert into ShowTime (MovieId, ShowDate,MaxRow,MaxCol) values (5,'2024-03-27 20:30',30,30)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId, BillId) values (0,'A',1,100000,6,2)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId, BillId) values (0,'A',2,100000,6,2)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId, BillId) values (0,'A',3,100000,6,3)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId, BillId) values (0,'A',4,100000,6,3)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId, BillId) values (0,'A',5,100000,6,3)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',6,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',7,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',8,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',9,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',10,100000,6)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',1,100000,6,4)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',2,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',3,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',4,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',5,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',6,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',7,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',8,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',9,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',10,100000,6)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',1,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',2,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',3,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',4,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',5,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',6,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',7,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',8,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',9,100000,6)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',10,100000,6)

INSERT INTO Ticket(IsAvailable, Row, Col, Price, BillId, ShowTimeId) VALUES (1, 'A', 1, 100000, 1, 1);
INSERT INTO Ticket(IsAvailable, Row, Col, Price, BillId, ShowTimeId) VALUES (1, 'A', 2, 100000, 1, 1);
INSERT INTO Ticket(IsAvailable, Row, Col, Price, BillId, ShowTimeId) VALUES (1, 'A', 3, 100000, 1, 1);
INSERT INTO Ticket(IsAvailable, Row, Col, Price, BillId, ShowTimeId) VALUES (1, 'A', 4, 100000, 1, 1);
INSERT INTO Ticket(IsAvailable, Row, Col, Price, BillId, ShowTimeId) VALUES (1, 'A', 5, 100000, 1, 1);


Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',1,100000,30,5)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',2,100000,30,5)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',3,100000,30,5)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',4,100000,30,5)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',5,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',6,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',7,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',8,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'A',9,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',10,100000,30)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',1,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',2,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',3,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',4,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',5,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',6,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',7,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',8,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',9,100000,30,8)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',10,100000,30,8)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',1,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',2,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',3,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',4,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',5,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',6,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',7,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',8,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',9,100000,30)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',10,100000,30)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',1,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',2,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',3,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',4,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',5,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',6,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',7,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',8,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',9,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (0,'A',10,100000,31)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',1,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',2,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',3,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',4,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',5,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',6,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',7,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',8,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',9,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',10,100000,31)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',1,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',2,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',3,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',4,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',5,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',6,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',7,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',8,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',9,100000,31)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',10,100000,31)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',1,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',2,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',3,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',4,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',5,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',6,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',7,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',8,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',9,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',10,100000,32)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',1,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',2,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',3,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',4,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',5,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',6,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',7,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',8,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',9,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',10,100000,32,9)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId, BillId) values (0,'C',1,100000,32,9)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',2,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',3,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',4,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',5,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',6,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',7,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',8,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',9,100000,32)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',10,100000,32)


Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',1,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',2,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',3,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',4,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',5,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',6,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',7,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',8,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',9,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',10,100000,40)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',1,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',2,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',3,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',4,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',5,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',6,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',7,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',8,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',9,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'B',10,100000,40)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',1,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',2,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',3,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',4,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',5,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',6,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',7,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',8,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',9,100000,40)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',10,100000,40)


Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',1,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',2,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',3,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',4,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',5,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',6,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',7,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',8,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',9,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'A',10,100000,36)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',1,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',2,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',3,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',4,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',5,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',6,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',7,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',8,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',9,100000,36,7)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId,BillId) values (0,'B',10,100000,36,7)

Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',1,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',2,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',3,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',4,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',5,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',6,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',7,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',8,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',9,100000,36)
Insert into Ticket(IsAvailable,Row,Col,Price,ShowTimeId) values (1,'C',10,100000,36)
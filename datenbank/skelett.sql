CREATE DATABASE telefonico;

create table user (
 
  email varchar(255),
  password varchar(255),
  
  primary key (email)
  
  );
  
  create table contacts (
	
	main_email varchar(255),
	friend_email varchar(255),
	

 foreign key (main_email)
	references user(email),
 foreign key (friend_email)
	references user(email)
  
  ) ;
  
  
  
  
  create table chat (
  
  main_email varchar(255),
    friend_email varchar(255), 
  message text,
  thetime timestamp,
    

 foreign key (main_email)
    references user(email),
 foreign key (friend_email)
    references user(email)
  ) ;
  
  
  
  
 -- Die Benutzer
insert into user (email, password) values ('admin@telefonico.com','1');
insert into user (email, password) values ('user@telefonico.com','1');
insert into user (email, password) values ('absender@gmail.com','1');
insert into user (email, password) values ('empfänger@gmail.com','1');
insert into user (email, password) values ('freund1@gmail.com','1');
insert into user (email, password) values ('freund2@gmail.com','1');
insert into user (email, password) values ('freund3@gmail.com','1');
insert into user (email, password) values ('freund4@gmail.com','1');


-- Die Kontakte

-- Kontakte von admin@telefonico.com
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','user@telefonico.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','absender@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','empfänger@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund1@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund2@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund3@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund4@gmail.com');

-- Kontakte von absender@gmail.com
insert into contacts (main_email, friend_email) values ('absender@gmail.com','empfänger@gmail.com');
insert into contacts (main_email, friend_email) values ('absender@gmail.com','freund1@gmail.com');

-- Kontakte von empfänger@gmail.com

insert into contacts (main_email, friend_email) values ('empfänger@gmail.com','absender@gmail.com');




-- Der Chat

insert into chat (main_email, friend_email, message, thetime) values ('absender@gmail.com','empfänger@gmail.com','Hallo empfänger', now());
CREATE SCHEMA `downloadlogs` ;

CREATE TABLE `downloadlogs`.`logs` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `host` VARCHAR(255) NULL,
  `arquivo` VARCHAR(255) NULL,
  `url` VARCHAR(255) NULL,
  PRIMARY KEY (`id`));

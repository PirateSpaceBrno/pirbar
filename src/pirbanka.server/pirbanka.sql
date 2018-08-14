-- phpMyAdmin SQL Dump
-- version 4.6.6deb4
-- https://www.phpmyadmin.net/
--
-- Počítač: localhost
-- Vytvořeno: Pon 13. srp 2018, 18:49
-- Verze serveru: 5.7.23
-- Verze PHP: 7.0.30-0+deb9u1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Databáze: `pirbanka`
--

-- --------------------------------------------------------

--
-- Struktura tabulky `accounts`
--

CREATE TABLE `accounts` (
  `id` int(11) NOT NULL,
  `identity` int(11) NOT NULL,
  `currency_id` int(11) NOT NULL,
  `market` tinyint(1) NOT NULL DEFAULT '0',
  `description` varchar(160) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL,
  `created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Zástupná struktura pro pohled `accounts_balances`
-- (See below for the actual view)
--
CREATE TABLE `accounts_balances` (
`id` int(11)
,`balance` decimal(59,18)
);

-- --------------------------------------------------------

--
-- Zástupná struktura pro pohled `accounts_view`
-- (See below for the actual view)
--
CREATE TABLE `accounts_view` (
`id` int(11)
,`identity` int(11)
,`currency_id` int(11)
,`market` tinyint(1)
,`description` varchar(160)
,`created` datetime
,`account_identifier` varchar(15)
,`balance` decimal(59,18)
);

-- --------------------------------------------------------

--
-- Struktura tabulky `authentications`
--

CREATE TABLE `authentications` (
  `id` int(11) NOT NULL,
  `identity` int(11) NOT NULL,
  `account` int(11) DEFAULT NULL,
  `content` varchar(1000) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL,
  `created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `expiration` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Struktura tabulky `currencies`
--

CREATE TABLE `currencies` (
  `id` int(11) NOT NULL,
  `name` varchar(50) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL,
  `shortname` varchar(3) CHARACTER SET utf8 COLLATE utf8_czech_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Zástupná struktura pro pohled `currencies_rates`
-- (See below for the actual view)
--
CREATE TABLE `currencies_rates` (
`id` int(11)
,`valid_since` datetime
,`rate` decimal(36,18) unsigned
);

-- --------------------------------------------------------

--
-- Zástupná struktura pro pohled `currencies_view`
-- (See below for the actual view)
--
CREATE TABLE `currencies_view` (
`id` int(11)
,`name` varchar(50)
,`shortname` varchar(3)
,`valid_since` datetime
,`rate` decimal(36,18)
);

-- --------------------------------------------------------

--
-- Struktura tabulky `exchange_rates`
--

CREATE TABLE `exchange_rates` (
  `id` int(11) NOT NULL,
  `currency` int(11) NOT NULL,
  `valid_since` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `rate` decimal(36,18) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Struktura tabulky `identities`
--

CREATE TABLE `identities` (
  `id` int(11) NOT NULL,
  `name` varchar(30) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL,
  `display_name` varchar(80) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL,
  `created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `admin` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Struktura tabulky `transactions`
--

CREATE TABLE `transactions` (
  `id` int(11) NOT NULL,
  `created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `source_account` int(11) NOT NULL,
  `target_account` int(11) NOT NULL,
  `amount` decimal(36,18) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Struktura pro pohled `accounts_balances`
--
DROP TABLE IF EXISTS `accounts_balances`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `accounts_balances`  AS  select `a`.`target_account` AS `id`,(sum(`a`.`amount`) - `b`.`amount`) AS `balance` from (`transactions` `a` join (select sum(`transactions`.`amount`) AS `amount`,`transactions`.`source_account` AS `source_account` from `transactions` group by `transactions`.`source_account`) `b` on((`b`.`source_account` = `a`.`target_account`))) group by `a`.`target_account` ;

-- --------------------------------------------------------

--
-- Struktura pro pohled `accounts_view`
--
DROP TABLE IF EXISTS `accounts_view`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `accounts_view`  AS  select `a`.`id` AS `id`,`a`.`identity` AS `identity`,`a`.`currency_id` AS `currency_id`,`a`.`market` AS `market`,`a`.`description` AS `description`,`a`.`created` AS `created`,concat(`a`.`id`,'/420') AS `account_identifier`,coalesce(`b`.`balance`,0) AS `balance` from (`accounts` `a` left join (select `accounts_balances`.`id` AS `id`,`accounts_balances`.`balance` AS `balance` from `accounts_balances`) `b` on((`b`.`id` = `a`.`id`))) ;

-- --------------------------------------------------------

--
-- Struktura pro pohled `currencies_rates`
--
DROP TABLE IF EXISTS `currencies_rates`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `currencies_rates`  AS  select `c`.`id` AS `id`,`d`.`valid_since` AS `valid_since`,`d`.`rate` AS `rate` from (`currencies` `c` join (select `t`.`currency` AS `currency`,`t`.`valid_since` AS `valid_since`,`t`.`rate` AS `rate` from `exchange_rates` `t` where (`t`.`valid_since` = (select max(`exchange_rates`.`valid_since`) from `exchange_rates` where (`exchange_rates`.`currency` = `t`.`currency`))) group by `t`.`currency`) `d` on((`d`.`currency` = `c`.`id`))) ;

-- --------------------------------------------------------

--
-- Struktura pro pohled `currencies_view`
--
DROP TABLE IF EXISTS `currencies_view`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `currencies_view`  AS  select `c`.`id` AS `id`,`c`.`name` AS `name`,`c`.`shortname` AS `shortname`,`r`.`valid_since` AS `valid_since`,coalesce(`r`.`rate`,1.0) AS `rate` from (`currencies` `c` left join (select `currencies_rates`.`id` AS `id`,`currencies_rates`.`valid_since` AS `valid_since`,`currencies_rates`.`rate` AS `rate` from `currencies_rates`) `r` on((`r`.`id` = `c`.`id`))) ;

--
-- Klíče pro exportované tabulky
--

--
-- Klíče pro tabulku `accounts`
--
ALTER TABLE `accounts`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_identity` (`identity`),
  ADD KEY `fk_currency_account` (`currency_id`);

--
-- Klíče pro tabulku `authentications`
--
ALTER TABLE `authentications`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `content` (`content`),
  ADD KEY `fk_identity_auth` (`identity`),
  ADD KEY `fk_account_auth` (`account`);

--
-- Klíče pro tabulku `currencies`
--
ALTER TABLE `currencies`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`),
  ADD UNIQUE KEY `shortname` (`shortname`);

--
-- Klíče pro tabulku `exchange_rates`
--
ALTER TABLE `exchange_rates`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_currency` (`currency`);

--
-- Klíče pro tabulku `identities`
--
ALTER TABLE `identities`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`),
  ADD UNIQUE KEY `display_name` (`display_name`);

--
-- Klíče pro tabulku `transactions`
--
ALTER TABLE `transactions`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_source_account` (`source_account`),
  ADD KEY `fk_target_account` (`target_account`);

--
-- AUTO_INCREMENT pro tabulky
--

--
-- AUTO_INCREMENT pro tabulku `accounts`
--
ALTER TABLE `accounts`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pro tabulku `authentications`
--
ALTER TABLE `authentications`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pro tabulku `currencies`
--
ALTER TABLE `currencies`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pro tabulku `exchange_rates`
--
ALTER TABLE `exchange_rates`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pro tabulku `identities`
--
ALTER TABLE `identities`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pro tabulku `transactions`
--
ALTER TABLE `transactions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- Omezení pro exportované tabulky
--

--
-- Omezení pro tabulku `accounts`
--
ALTER TABLE `accounts`
  ADD CONSTRAINT `fk_currency_account` FOREIGN KEY (`currency_id`) REFERENCES `currencies` (`id`),
  ADD CONSTRAINT `fk_identity` FOREIGN KEY (`identity`) REFERENCES `identities` (`id`);

--
-- Omezení pro tabulku `authentications`
--
ALTER TABLE `authentications`
  ADD CONSTRAINT `fk_account_auth` FOREIGN KEY (`account`) REFERENCES `accounts` (`id`),
  ADD CONSTRAINT `fk_identity_auth` FOREIGN KEY (`identity`) REFERENCES `identities` (`id`);

--
-- Omezení pro tabulku `exchange_rates`
--
ALTER TABLE `exchange_rates`
  ADD CONSTRAINT `fk_currency` FOREIGN KEY (`currency`) REFERENCES `currencies` (`id`);

--
-- Omezení pro tabulku `transactions`
--
ALTER TABLE `transactions`
  ADD CONSTRAINT `fk_source_account` FOREIGN KEY (`source_account`) REFERENCES `accounts` (`id`),
  ADD CONSTRAINT `fk_target_account` FOREIGN KEY (`target_account`) REFERENCES `accounts` (`id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

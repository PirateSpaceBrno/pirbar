-- phpMyAdmin SQL Dump
-- version 4.6.6deb4
-- https://www.phpmyadmin.net/
--
-- Počítač: localhost
-- Vytvořeno: Stř 08. srp 2018, 16:42
-- Verze serveru: 5.7.23
-- Verze PHP: 7.0.30-0+deb9u1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Databáze: `brmbar`
--

-- --------------------------------------------------------

--
-- Struktura tabulky `accounts`
--

CREATE TABLE `accounts` (
  `id` int(11) NOT NULL,
  `identity` int(11) NOT NULL,
  `currency` int(11) NOT NULL,
  `transparent` tinyint(1) NOT NULL DEFAULT '0',
  `description` varchar(160) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Zástupná struktura pro pohled `accounts_balances`
-- (See below for the actual view)
--
CREATE TABLE `accounts_balances` (
`account` int(11)
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
  `content` varchar(100) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL
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
-- Zástupná struktura pro pohled `currencies_view`
-- (See below for the actual view)
--
CREATE TABLE `currencies_view` (
`id` int(11)
,`name` varchar(50)
,`shortname` varchar(3)
,`valid_since` datetime
,`rate` decimal(36,18) unsigned
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
  `name` varchar(60) CHARACTER SET utf8 COLLATE utf8_czech_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Struktura tabulky `transactions`
--

CREATE TABLE `transactions` (
  `id` int(11) NOT NULL,
  `time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `source_account` int(11) NOT NULL,
  `target_account` int(11) NOT NULL,
  `amount` decimal(36,18) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Struktura pro pohled `accounts_balances`
--
DROP TABLE IF EXISTS `accounts_balances`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `accounts_balances`  AS  select `a`.`target_account` AS `account`,(sum(`a`.`amount`) - `b`.`amount`) AS `balance` from (`transactions` `a` join (select sum(`transactions`.`amount`) AS `amount`,`transactions`.`source_account` AS `source_account` from `transactions` group by `transactions`.`source_account`) `b` on((`b`.`source_account` = `a`.`target_account`))) group by `a`.`target_account` ;

-- --------------------------------------------------------

--
-- Struktura pro pohled `currencies_view`
--
DROP TABLE IF EXISTS `currencies_view`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `currencies_view`  AS  select `c`.`id` AS `id`,`c`.`name` AS `name`,`c`.`shortname` AS `shortname`,`d`.`valid_since` AS `valid_since`,`d`.`rate` AS `rate` from (`currencies` `c` join (select `t`.`currency` AS `currency`,`t`.`valid_since` AS `valid_since`,`t`.`rate` AS `rate` from `exchange_rates` `t` where (`t`.`valid_since` = (select max(`exchange_rates`.`valid_since`) from `exchange_rates` where (`exchange_rates`.`currency` = `t`.`currency`))) group by `t`.`currency`) `d` on((`d`.`currency` = `c`.`id`))) ;

--
-- Klíče pro exportované tabulky
--

--
-- Klíče pro tabulku `accounts`
--
ALTER TABLE `accounts`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_identity` (`identity`),
  ADD KEY `fk_currency_account` (`currency`);

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
  ADD PRIMARY KEY (`id`);

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
  ADD UNIQUE KEY `name` (`name`);

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
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
  ADD CONSTRAINT `fk_currency_account` FOREIGN KEY (`currency`) REFERENCES `currencies` (`id`),
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

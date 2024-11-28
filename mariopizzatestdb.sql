-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Nov 28, 2024 at 02:08 PM
-- Server version: 5.7.36
-- PHP Version: 7.4.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `mariopizzatestdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `ingredients`
--

DROP TABLE IF EXISTS `ingredients`;
CREATE TABLE IF NOT EXISTS `ingredients` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `purchase_price` decimal(5,2) DEFAULT NULL,
  `finishing_ingredient` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `ingredients`
--

INSERT INTO `ingredients` (`id`, `name`, `purchase_price`, `finishing_ingredient`) VALUES
(1, 'Tomato Sauce', '0.50', 0),
(2, 'Cheese', '1.00', 0),
(3, 'Pepperoni', '1.50', 0),
(4, 'Mushrooms', '0.75', 0),
(5, 'Onions', '0.50', 0),
(6, 'Sausage', '1.50', 0),
(7, 'Bacon', '1.75', 0),
(8, 'Black Olives', '0.75', 0),
(9, 'Green Peppers', '0.50', 0),
(10, 'Pineapple', '1.00', 0),
(11, 'Spinach', '0.75', 0);

-- --------------------------------------------------------

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
CREATE TABLE IF NOT EXISTS `inventory` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `date` date DEFAULT NULL,
  `value` decimal(5,2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `inventory_ingredients`
--

DROP TABLE IF EXISTS `inventory_ingredients`;
CREATE TABLE IF NOT EXISTS `inventory_ingredients` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `inventoryID` int(11) DEFAULT NULL,
  `ingredientID` int(11) DEFAULT NULL,
  `amount` int(11) DEFAULT NULL,
  `value` decimal(10,0) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `InventoryIngredients` (`inventoryID`),
  KEY `IngredientsInventory` (`ingredientID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
CREATE TABLE IF NOT EXISTS `orders` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `total` decimal(5,2) DEFAULT NULL,
  `date` date DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `orders_pizzas`
--

DROP TABLE IF EXISTS `orders_pizzas`;
CREATE TABLE IF NOT EXISTS `orders_pizzas` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pizzaID` int(11) DEFAULT NULL,
  `orderID` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `PizzasOrders` (`pizzaID`),
  KEY `OrdersPizzas` (`orderID`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `orders_pizza_customizations`
--

DROP TABLE IF EXISTS `orders_pizza_customizations`;
CREATE TABLE IF NOT EXISTS `orders_pizza_customizations` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ingredientID` int(11) DEFAULT NULL,
  `order_pizzaID` int(11) DEFAULT NULL,
  `modification_type` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `PizzaOrderCustomization` (`order_pizzaID`),
  KEY `IngredientsOrderCustomization` (`ingredientID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `pizzas`
--

DROP TABLE IF EXISTS `pizzas`;
CREATE TABLE IF NOT EXISTS `pizzas` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `price` decimal(5,2) DEFAULT NULL,
  `sizeID` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `PizzasSizes` (`sizeID`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `pizzas`
--

INSERT INTO `pizzas` (`id`, `name`, `price`, `sizeID`) VALUES
(1, 'Margherita', '8.00', 1),
(2, 'Margherita', '9.00', 2),
(3, 'Margherita', '10.00', 3),
(4, 'Margherita', '11.00', 4),
(5, 'Pepperoni', '9.00', 1),
(6, 'Pepperoni', '10.00', 2),
(7, 'Pepperoni', '11.00', 3),
(8, 'Pepperoni', '12.00', 4),
(9, 'BBQ Chicken', '10.00', 1),
(10, 'BBQ Chicken', '11.00', 2),
(11, 'BBQ Chicken', '12.00', 3),
(12, 'BBQ Chicken', '13.00', 4),
(13, 'Hawaiian', '9.50', 1),
(14, 'Hawaiian', '10.50', 2),
(15, 'Hawaiian', '11.50', 3),
(16, 'Hawaiian', '12.50', 4),
(17, 'Veggie', '8.50', 1),
(18, 'Veggie', '9.50', 2),
(19, 'Veggie', '10.50', 3),
(20, 'Veggie', '11.50', 4),
(21, 'Meat Lovers', '11.00', 1),
(22, 'Meat Lovers', '12.00', 2),
(23, 'Meat Lovers', '13.00', 3),
(24, 'Meat Lovers', '14.00', 4),
(25, 'Supreme', '12.00', 1),
(26, 'Supreme', '13.00', 2),
(27, 'Supreme', '14.00', 3),
(28, 'Supreme', '15.00', 4),
(29, 'Buffalo Chicken', '10.50', 1),
(30, 'Buffalo Chicken', '11.50', 2),
(31, 'Buffalo Chicken', '12.50', 3),
(32, 'Buffalo Chicken', '13.50', 4),
(33, 'Four Cheese', '9.00', 1),
(34, 'Four Cheese', '10.00', 2),
(35, 'Four Cheese', '11.00', 3),
(36, 'Four Cheese', '12.00', 4),
(37, 'Spinach Alfredo', '9.50', 1),
(38, 'Spinach Alfredo', '10.50', 2),
(39, 'Spinach Alfredo', '11.50', 3),
(40, 'Spinach Alfredo', '12.50', 4);

-- --------------------------------------------------------

--
-- Table structure for table `pizzas_ingredients`
--

DROP TABLE IF EXISTS `pizzas_ingredients`;
CREATE TABLE IF NOT EXISTS `pizzas_ingredients` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pizzaID` int(11) DEFAULT NULL,
  `ingredientID` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `PizzaIngredients` (`pizzaID`),
  KEY `IngredientsPizza` (`ingredientID`)
) ENGINE=InnoDB AUTO_INCREMENT=161 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `pizzas_ingredients`
--

INSERT INTO `pizzas_ingredients` (`id`, `pizzaID`, `ingredientID`) VALUES
(1, 1, 1),
(2, 1, 2),
(3, 2, 1),
(4, 2, 2),
(5, 3, 1),
(6, 3, 2),
(7, 4, 1),
(8, 4, 2),
(9, 5, 1),
(10, 5, 2),
(11, 5, 3),
(12, 6, 1),
(13, 6, 2),
(14, 6, 3),
(15, 7, 1),
(16, 7, 2),
(17, 7, 3),
(18, 8, 1),
(19, 8, 2),
(20, 8, 3),
(21, 9, 1),
(22, 9, 2),
(23, 9, 6),
(24, 10, 1),
(25, 10, 2),
(26, 10, 6),
(27, 11, 1),
(28, 11, 2),
(29, 11, 6),
(30, 12, 1),
(31, 12, 2),
(32, 12, 6),
(33, 13, 1),
(34, 13, 2),
(35, 13, 10),
(36, 14, 1),
(37, 14, 2),
(38, 14, 10),
(39, 15, 1),
(40, 15, 2),
(41, 15, 10),
(42, 16, 1),
(43, 16, 2),
(44, 16, 10),
(45, 17, 1),
(46, 17, 2),
(47, 17, 4),
(48, 17, 5),
(49, 17, 8),
(50, 17, 9),
(51, 17, 11),
(52, 18, 1),
(53, 18, 2),
(54, 18, 4),
(55, 18, 5),
(56, 18, 8),
(57, 18, 9),
(58, 18, 11),
(59, 19, 1),
(60, 19, 2),
(61, 19, 4),
(62, 19, 5),
(63, 19, 8),
(64, 19, 9),
(65, 19, 11),
(66, 20, 1),
(67, 20, 2),
(68, 20, 4),
(69, 20, 5),
(70, 20, 8),
(71, 20, 9),
(72, 20, 11),
(73, 21, 1),
(74, 21, 2),
(75, 21, 3),
(76, 21, 6),
(77, 21, 7),
(78, 22, 1),
(79, 22, 2),
(80, 22, 3),
(81, 22, 6),
(82, 22, 7),
(83, 23, 1),
(84, 23, 2),
(85, 23, 3),
(86, 23, 6),
(87, 23, 7),
(88, 24, 1),
(89, 24, 2),
(90, 24, 3),
(91, 24, 6),
(92, 24, 7),
(93, 25, 1),
(94, 25, 2),
(95, 25, 3),
(96, 25, 4),
(97, 25, 5),
(98, 25, 6),
(99, 25, 7),
(100, 25, 8),
(101, 25, 9),
(102, 26, 1),
(103, 26, 2),
(104, 26, 3),
(105, 26, 4),
(106, 26, 5),
(107, 26, 6),
(108, 26, 7),
(109, 26, 8),
(110, 26, 9),
(111, 27, 1),
(112, 27, 2),
(113, 27, 3),
(114, 27, 4),
(115, 27, 5),
(116, 27, 6),
(117, 27, 7),
(118, 27, 8),
(119, 27, 9),
(120, 28, 1),
(121, 28, 2),
(122, 28, 3),
(123, 28, 4),
(124, 28, 5),
(125, 28, 6),
(126, 28, 7),
(127, 28, 8),
(128, 28, 9),
(129, 29, 1),
(130, 29, 2),
(131, 29, 6),
(132, 30, 1),
(133, 30, 2),
(134, 30, 6),
(135, 31, 1),
(136, 31, 2),
(137, 31, 6),
(138, 32, 1),
(139, 32, 2),
(140, 32, 6),
(141, 33, 1),
(142, 33, 2),
(143, 34, 1),
(144, 34, 2),
(145, 35, 1),
(146, 35, 2),
(147, 36, 1),
(148, 36, 2),
(149, 37, 1),
(150, 37, 2),
(151, 37, 11),
(152, 38, 1),
(153, 38, 2),
(154, 38, 11),
(155, 39, 1),
(156, 39, 2),
(157, 39, 11),
(158, 40, 1),
(159, 40, 2),
(160, 40, 11);

-- --------------------------------------------------------

--
-- Table structure for table `sizes`
--

DROP TABLE IF EXISTS `sizes`;
CREATE TABLE IF NOT EXISTS `sizes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `sizes`
--

INSERT INTO `sizes` (`id`, `name`) VALUES
(1, 'Small'),
(2, 'Medium'),
(3, 'Large'),
(4, 'Extra Large');

--
-- Constraints for dumped tables
--

--
-- Constraints for table `inventory_ingredients`
--
ALTER TABLE `inventory_ingredients`
  ADD CONSTRAINT `IngredientsInventory` FOREIGN KEY (`ingredientID`) REFERENCES `ingredients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `InventoryIngredients` FOREIGN KEY (`inventoryID`) REFERENCES `inventory` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `orders_pizzas`
--
ALTER TABLE `orders_pizzas`
  ADD CONSTRAINT `OrdersPizzas` FOREIGN KEY (`orderID`) REFERENCES `orders` (`id`),
  ADD CONSTRAINT `PizzasOrders` FOREIGN KEY (`pizzaID`) REFERENCES `pizzas` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `orders_pizza_customizations`
--
ALTER TABLE `orders_pizza_customizations`
  ADD CONSTRAINT `IngredientsOrderCustomization` FOREIGN KEY (`ingredientID`) REFERENCES `ingredients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `PizzaOrderCustomization` FOREIGN KEY (`order_pizzaID`) REFERENCES `orders_pizzas` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `pizzas`
--
ALTER TABLE `pizzas`
  ADD CONSTRAINT `PizzasSizes` FOREIGN KEY (`sizeID`) REFERENCES `sizes` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `pizzas_ingredients`
--
ALTER TABLE `pizzas_ingredients`
  ADD CONSTRAINT `IngredientsPizza` FOREIGN KEY (`ingredientID`) REFERENCES `ingredients` (`id`),
  ADD CONSTRAINT `PizzaIngredients` FOREIGN KEY (`pizzaID`) REFERENCES `pizzas` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

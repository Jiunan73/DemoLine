CREATE TABLE IF NOT EXISTS `battery` (
  `flag` int(11) NOT NULL,
  `SerialNo` varchar(50) NOT NULL,
  `CarNo` int(11) NOT NULL,
  `SOH` int(11) NOT NULL,
  `CHG_AH` int(11) NOT NULL,
  `DSG_AH` int(11) NOT NULL,
  `CYCLE` int(11) NOT NULL,
  `LC_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '最後充電時間',
  `FirstTime` datetime NOT NULL COMMENT '第一次充電時間',
  `note` varchar(50) NOT NULL,
  `shipment_time` datetime NOT NULL,
  `offline_time` datetime NOT NULL,
  PRIMARY KEY (`SerialNo`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- 转存表中的数据 `battery`
--

INSERT INTO `battery` (`flag`, `SerialNo`, `CarNo`, `SOH`, `CHG_AH`, `DSG_AH`, `CYCLE`, `LC_time`, `FirstTime`, `note`, `shipment_time`, `offline_time`) VALUES
(0, '074-21221600002', 9001, 100, 126377, 125358, 262, '2024-01-24 01:32:00', '0000-00-00 00:00:00', '', '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(0, '074-22223200002', 9001, 100, 727, 511, 1, '2024-01-24 01:51:30', '0000-00-00 00:00:00', '', '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(0, '074-22223200006', 105, 100, 0, 0, 0, '2024-01-24 01:32:00', '0000-00-00 00:00:00', '', '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(0, '084-21224100021', 9001, 100, 2408, 2657, 3, '2024-01-24 01:32:00', '0000-00-00 00:00:00', '', '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(0, '088-21223000002', 3, 100, 0, 0, 0, '2024-01-24 01:30:07', '0000-00-00 00:00:00', '', '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(0, 'HIK_6680', 6680, 100, 0, 0, 0, '2024-01-24 01:51:30', '0000-00-00 00:00:00', '', '0000-00-00 00:00:00', '0000-00-00 00:00:00');

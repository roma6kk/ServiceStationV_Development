-- Вставка всех 20 записей с локализацией (только ServiceType на русском)
INSERT INTO Services (
    ServiceName, ServiceNameEN,
    SmallDescription, SmallDescriptionEN,
    LargeDescription, LargeDescriptionEN,
    Price, ImageSrc, ServiceType
) 
VALUES
-- 1. Диагностика двигателя
('Диагностика двигателя', 'Engine diagnostics',
 'Комплексная проверка двигателя', 'Comprehensive engine check',
 'Полная диагностика состояния двигателя с использованием современного оборудования', 'Full engine condition diagnostics using modern equipment',
 1500, 'pack://siteoforigin:,,,/images/ServicesImages/EnigneDiagnostic.jpeg', 'Диагностика'),

-- 2. Обслуживание двигателя
('Обслуживание двигателя', 'Engine maintenance',
 'Стандартное обслуживание двигателя', 'Standard engine maintenance',
 'Комплекс работ по поддержанию работоспособности двигателя', 'A set of works to maintain engine operability',
 3000, 'pack://siteoforigin:,,,/images/ServicesImages/EngineCheck.jpeg', 'Обслуживание'),

-- 3. Замена масла
('Замена масла', 'Oil change',
 'Замена моторного масла и фильтра', 'Replacement of engine oil and filter',
 'Замена моторного масла, масляного фильтра и проверка уровня технических жидкостей', 'Replacement of engine oil, oil filter, and checking the level of technical fluids',
 1200, 'pack://siteoforigin:,,,/images/ServicesImages/OilChange.jpeg', 'Обслуживание'),

-- 4. Замена свечей зажигания
('Замена свечей зажигания', 'Spark plug replacement',
 'Замена свечей зажигания', 'Replacement of spark plugs',
 'Демонтаж старых и установка новых свечей зажигания с проверкой зазоров', 'Dismantling old and installing new spark plugs with gap checks',
 800, 'pack://siteoforigin:,,,/images/ServicesImages/GlowPlugChange.jpeg', 'Двигатель'),

-- 5. Замена ремня ГРМ
('Замена ремня ГРМ', 'Timing belt replacement',
 'Замена ремня газораспределительного механизма', 'Replacement of the timing belt mechanism',
 'Замена ремня ГРМ с проверкой состояния натяжителей и роликов', 'Replacement of the timing belt with a check of tensioners and rollers',
 4500, 'pack://siteoforigin:,,,/images/ServicesImages/TimingBeltChange.jpeg', 'Двигатель'),

-- 6. Замена форсунок
('Замена форсунок', 'Injector replacement',
 'Замена топливных форсунок', 'Replacement of fuel injectors',
 'Демонтаж старых и установка новых топливных форсунок с диагностикой системы впрыска', 'Dismantling old and installing new fuel injectors with injection system diagnostics',
 3200, 'pack://siteoforigin:,,,/images/ServicesImages/NozzleChange.jpeg', 'Двигатель'),

-- 7. Ремонт радиатора
('Ремонт радиатора', 'Radiator repair',
 'Ремонт или замена радиатора', 'Repair or replacement of the radiator',
 'Диагностика, ремонт или замена радиатора охлаждения двигателя', 'Diagnosis, repair, or replacement of the engine cooling radiator',
 2800, 'pack://siteoforigin:,,,/images/ServicesImages/RadiatorCheck.jpeg', 'Охлаждение'),

-- 8. Система охлаждения
('Система охлаждения', 'Cooling system',
 'Проверка системы охлаждения', 'Checking the cooling system',
 'Комплексная диагностика системы охлаждения двигателя на предмет утечек и эффективности работы', 'Comprehensive diagnostics of the engine cooling system for leaks and performance',
 1300, 'pack://siteoforigin:,,,/images/ServicesImages/CoolingSystemDiagnostic.jpeg', 'Диагностика'),

-- 9. Обслуживание подвески
('Обслуживание подвески', 'Suspension maintenance',
 'Комплексное обслуживание подвески', 'Comprehensive suspension maintenance',
 'Диагностика и обслуживание элементов подвески автомобиля', 'Diagnosis and maintenance of vehicle suspension components',
 2500, 'pack://siteoforigin:,,,/images/ServicesImages/SuspensionCheck.jpeg', 'Подвеска'),

-- 10. Правка дисков
('Правка дисков', 'Disk alignment',
 'Восстановление геометрии дисков', 'Restoration of disk geometry',
 'Правка стальных или легкосплавных дисков на специальном станке', 'Alignment of steel or alloy disks on a special machine',
 900, 'pack://siteoforigin:,,,/images/ServicesImages/DiskEdit.jpeg', 'Колеса'),

-- 11. Протектирование шин
('Протектирование шин', 'Tire tread treatment',
 'Нанесение защитного слоя на шины', 'Application of a protective layer on tires',
 'Обработка шин специальным составом для защиты от внешних воздействий', 'Treatment of tires with a special compound for protection against external influences',
 600, 'pack://siteoforigin:,,,/images/ServicesImages/TireTread.jpeg', 'Колеса'),

-- 12. Замена тормозных колодок
('Замена тормозных колодок', 'Brake pad replacement',
 'Замена передних или задних тормозных колодок', 'Replacement of front or rear brake pads',
 'Демонтаж изношенных и установка новых тормозных колодок с диагностикой состояния тормозных дисков', 'Dismantling worn-out and installing new brake pads with a diagnostic check of brake disc condition',
 1800, 'pack://siteoforigin:,,,/images/ServicesImages/PadsCheck.jpeg', 'Тормоза'),

-- 13. Замена тормозных дисков
('Замена тормозных дисков', 'Brake disc replacement',
 'Замена передних или задних тормозных дисков', 'Replacement of front or rear brake discs',
 'Демонтаж старых и установка новых тормозных дисков вместе с заменой колодок', 'Dismantling old and installing new brake discs along with pad replacement',
 3500, 'pack://siteoforigin:,,,/images/ServicesImages/BrakeDiskCheck.jpeg', 'Тормоза'),

-- 14. Улучшение оптики (тюнинг)
('Улучшение оптики (тюнинг)', 'Optics improvement (tuning)',
 'Тюнинг фар и фонарей', 'Tuning headlights and taillights',
 'Установка светодиодных элементов, тонировка или другие работы по улучшению внешнего вида оптики', 'Installation of LED elements, tinting, or other works to improve the appearance of optics',
 4200, 'pack://siteoforigin:,,,/images/ServicesImages/OpticCheck.jpeg', 'Тюнинг'),

-- 15. Обновление поворотников
('Обновление поворотников', 'Turn signal upgrade',
 'Модернизация поворотников', 'Upgrade of turn signals',
 'Замена стандартных поворотников на светодиодные или другие улучшенные варианты', 'Replacement of standard turn signals with LED or other improved options',
 1500, 'pack://siteoforigin:,,,/images/ServicesImages/TurnSignals.jpeg', 'Тюнинг'),

-- 16. Установка громкоговорителя
('Установка громкоговорителя', 'Megaphone installation',
 'Монтаж звукового сигнала', 'Installation of a horn',
 'Установка дополнительного или замены штатного звукового сигнала', 'Installation of an additional or replacement of the standard horn',
 900, 'pack://siteoforigin:,,,/images/ServicesImages/MegaphoneInstallation.jpeg', 'Тюнинг'),

-- 17. Проводка
('Проводка', 'Wiring',
 'Работы с электропроводкой', 'Electrical wiring works',
 'Диагностика и ремонт электропроводки автомобиля', 'Diagnosis and repair of vehicle electrical wiring',
 2100, 'pack://siteoforigin:,,,/images/ServicesImages/WiringDiagnostic.jpeg', 'Диагностика'),

-- 18. Установка сигнализации
('Установка сигнализации', 'Alarm installation',
 'Монтаж охранной системы', 'Installation of a security system',
 'Установка автомобильной сигнализации с настройкой всех функций', 'Installation of a car alarm with configuration of all functions',
 3800, 'pack://siteoforigin:,,,/images/ServicesImages/AlarmInstallation.jpeg', 'Тюнинг'),

-- 19. Установка шторок
('Установка шторок', 'Blinds installation',
 'Монтаж солнцезащитных шторок', 'Installation of sunshades',
 'Установка солнцезащитных шторок на окна автомобиля', 'Installation of sunshades on car windows',
 1700, 'pack://siteoforigin:,,,/images/ServicesImages/BlindsInstallation.jpeg', 'Тюнинг'),

-- 20. Улучшение выхлопа
('Улучшение выхлопа', 'Exhaust improvement',
 'Тюнинг выхлопной системы', 'Tuning the exhaust system',
 'Замена элементов выхлопной системы для улучшения внешнего вида или звука', 'Replacement of exhaust system components to improve appearance or sound',
 5000, 'pack://siteoforigin:,,,/images/ServicesImages/Exhaust.jpeg', 'Тюнинг');
-- ������� ���� 20 ������� � ������������ (������ ServiceType �� �������)
INSERT INTO Services (
    ServiceName, ServiceNameEN,
    SmallDescription, SmallDescriptionEN,
    LargeDescription, LargeDescriptionEN,
    Price, ImageSrc, ServiceType
) 
VALUES
-- 1. ����������� ���������
('����������� ���������', 'Engine diagnostics',
 '����������� �������� ���������', 'Comprehensive engine check',
 '������ ����������� ��������� ��������� � �������������� ������������ ������������', 'Full engine condition diagnostics using modern equipment',
 1500, 'pack://siteoforigin:,,,/images/ServicesImages/EnigneDiagnostic.jpeg', '�����������'),

-- 2. ������������ ���������
('������������ ���������', 'Engine maintenance',
 '����������� ������������ ���������', 'Standard engine maintenance',
 '�������� ����� �� ����������� ����������������� ���������', 'A set of works to maintain engine operability',
 3000, 'pack://siteoforigin:,,,/images/ServicesImages/EngineCheck.jpeg', '������������'),

-- 3. ������ �����
('������ �����', 'Oil change',
 '������ ��������� ����� � �������', 'Replacement of engine oil and filter',
 '������ ��������� �����, ��������� ������� � �������� ������ ����������� ���������', 'Replacement of engine oil, oil filter, and checking the level of technical fluids',
 1200, 'pack://siteoforigin:,,,/images/ServicesImages/OilChange.jpeg', '������������'),

-- 4. ������ ������ ���������
('������ ������ ���������', 'Spark plug replacement',
 '������ ������ ���������', 'Replacement of spark plugs',
 '�������� ������ � ��������� ����� ������ ��������� � ��������� �������', 'Dismantling old and installing new spark plugs with gap checks',
 800, 'pack://siteoforigin:,,,/images/ServicesImages/GlowPlugChange.jpeg', '���������'),

-- 5. ������ ����� ���
('������ ����� ���', 'Timing belt replacement',
 '������ ����� ���������������������� ���������', 'Replacement of the timing belt mechanism',
 '������ ����� ��� � ��������� ��������� ����������� � �������', 'Replacement of the timing belt with a check of tensioners and rollers',
 4500, 'pack://siteoforigin:,,,/images/ServicesImages/TimingBeltChange.jpeg', '���������'),

-- 6. ������ ��������
('������ ��������', 'Injector replacement',
 '������ ��������� ��������', 'Replacement of fuel injectors',
 '�������� ������ � ��������� ����� ��������� �������� � ������������ ������� �������', 'Dismantling old and installing new fuel injectors with injection system diagnostics',
 3200, 'pack://siteoforigin:,,,/images/ServicesImages/NozzleChange.jpeg', '���������'),

-- 7. ������ ���������
('������ ���������', 'Radiator repair',
 '������ ��� ������ ���������', 'Repair or replacement of the radiator',
 '�����������, ������ ��� ������ ��������� ���������� ���������', 'Diagnosis, repair, or replacement of the engine cooling radiator',
 2800, 'pack://siteoforigin:,,,/images/ServicesImages/RadiatorCheck.jpeg', '����������'),

-- 8. ������� ����������
('������� ����������', 'Cooling system',
 '�������� ������� ����������', 'Checking the cooling system',
 '����������� ����������� ������� ���������� ��������� �� ������� ������ � ������������� ������', 'Comprehensive diagnostics of the engine cooling system for leaks and performance',
 1300, 'pack://siteoforigin:,,,/images/ServicesImages/CoolingSystemDiagnostic.jpeg', '�����������'),

-- 9. ������������ ��������
('������������ ��������', 'Suspension maintenance',
 '����������� ������������ ��������', 'Comprehensive suspension maintenance',
 '����������� � ������������ ��������� �������� ����������', 'Diagnosis and maintenance of vehicle suspension components',
 2500, 'pack://siteoforigin:,,,/images/ServicesImages/SuspensionCheck.jpeg', '��������'),

-- 10. ������ ������
('������ ������', 'Disk alignment',
 '�������������� ��������� ������', 'Restoration of disk geometry',
 '������ �������� ��� ������������� ������ �� ����������� ������', 'Alignment of steel or alloy disks on a special machine',
 900, 'pack://siteoforigin:,,,/images/ServicesImages/DiskEdit.jpeg', '������'),

-- 11. ��������������� ���
('��������������� ���', 'Tire tread treatment',
 '��������� ��������� ���� �� ����', 'Application of a protective layer on tires',
 '��������� ��� ����������� �������� ��� ������ �� ������� �����������', 'Treatment of tires with a special compound for protection against external influences',
 600, 'pack://siteoforigin:,,,/images/ServicesImages/TireTread.jpeg', '������'),

-- 12. ������ ��������� �������
('������ ��������� �������', 'Brake pad replacement',
 '������ �������� ��� ������ ��������� �������', 'Replacement of front or rear brake pads',
 '�������� ���������� � ��������� ����� ��������� ������� � ������������ ��������� ��������� ������', 'Dismantling worn-out and installing new brake pads with a diagnostic check of brake disc condition',
 1800, 'pack://siteoforigin:,,,/images/ServicesImages/PadsCheck.jpeg', '�������'),

-- 13. ������ ��������� ������
('������ ��������� ������', 'Brake disc replacement',
 '������ �������� ��� ������ ��������� ������', 'Replacement of front or rear brake discs',
 '�������� ������ � ��������� ����� ��������� ������ ������ � ������� �������', 'Dismantling old and installing new brake discs along with pad replacement',
 3500, 'pack://siteoforigin:,,,/images/ServicesImages/BrakeDiskCheck.jpeg', '�������'),

-- 14. ��������� ������ (������)
('��������� ������ (������)', 'Optics improvement (tuning)',
 '������ ��� � �������', 'Tuning headlights and taillights',
 '��������� ������������ ���������, ��������� ��� ������ ������ �� ��������� �������� ���� ������', 'Installation of LED elements, tinting, or other works to improve the appearance of optics',
 4200, 'pack://siteoforigin:,,,/images/ServicesImages/OpticCheck.jpeg', '������'),

-- 15. ���������� ������������
('���������� ������������', 'Turn signal upgrade',
 '������������ ������������', 'Upgrade of turn signals',
 '������ ����������� ������������ �� ������������ ��� ������ ���������� ��������', 'Replacement of standard turn signals with LED or other improved options',
 1500, 'pack://siteoforigin:,,,/images/ServicesImages/TurnSignals.jpeg', '������'),

-- 16. ��������� ����������������
('��������� ����������������', 'Megaphone installation',
 '������ ��������� �������', 'Installation of a horn',
 '��������� ��������������� ��� ������ �������� ��������� �������', 'Installation of an additional or replacement of the standard horn',
 900, 'pack://siteoforigin:,,,/images/ServicesImages/MegaphoneInstallation.jpeg', '������'),

-- 17. ��������
('��������', 'Wiring',
 '������ � ����������������', 'Electrical wiring works',
 '����������� � ������ ��������������� ����������', 'Diagnosis and repair of vehicle electrical wiring',
 2100, 'pack://siteoforigin:,,,/images/ServicesImages/WiringDiagnostic.jpeg', '�����������'),

-- 18. ��������� ������������
('��������� ������������', 'Alarm installation',
 '������ �������� �������', 'Installation of a security system',
 '��������� ������������� ������������ � ���������� ���� �������', 'Installation of a car alarm with configuration of all functions',
 3800, 'pack://siteoforigin:,,,/images/ServicesImages/AlarmInstallation.jpeg', '������'),

-- 19. ��������� ������
('��������� ������', 'Blinds installation',
 '������ �������������� ������', 'Installation of sunshades',
 '��������� �������������� ������ �� ���� ����������', 'Installation of sunshades on car windows',
 1700, 'pack://siteoforigin:,,,/images/ServicesImages/BlindsInstallation.jpeg', '������'),

-- 20. ��������� �������
('��������� �������', 'Exhaust improvement',
 '������ ��������� �������', 'Tuning the exhaust system',
 '������ ��������� ��������� ������� ��� ��������� �������� ���� ��� �����', 'Replacement of exhaust system components to improve appearance or sound',
 5000, 'pack://siteoforigin:,,,/images/ServicesImages/Exhaust.jpeg', '������');
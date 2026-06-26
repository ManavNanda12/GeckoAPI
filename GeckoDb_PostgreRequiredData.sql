-- =====================================================
-- DemoWebApp - PostgreSQL Seed Data
-- Run AFTER the schema migration script
-- Target: neondb on GeckoServer (pgAdmin)
-- =====================================================
-- INSTRUCTIONS:
--   1. Open pgAdmin → GeckoServer → neondb → Query Tool
--   2. Paste this entire script
--   3. Press F5 to execute
-- =====================================================

-- =====================================================
-- 1. SITE POLICIES (Required by checkout & cart logic)
-- =====================================================
INSERT INTO "SitePolicies" ("PolicyName", "PolicyDescription")
VALUES
    ('TaxPercentage', '0.05'),
    ('ShippingPolicy', 'Standard shipping applies to all orders. Free shipping on orders above $50.'),
    ('ReturnPolicy', 'Items can be returned within 30 days of delivery in original condition.'),
    ('PrivacyPolicy', 'We respect your privacy and protect your personal data in accordance with applicable laws.'),
    ('TermsAndConditions', 'By using this website, you agree to our terms and conditions.')
ON CONFLICT DO NOTHING;

-- =====================================================
-- 2. COUNTRY MASTER
-- =====================================================
INSERT INTO "CountryMaster" ("CountryId", "CountryName")
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'India'),
    (2, 'United States'),
    (3, 'United Kingdom'),
    (4, 'Canada'),
    (5, 'Australia'),
    (6, 'Germany'),
    (7, 'France'),
    (8, 'Japan'),
    (9, 'Singapore'),
    (10, 'United Arab Emirates')
ON CONFLICT DO NOTHING;

-- Reset sequence after explicit ID inserts
SELECT setval(pg_get_serial_sequence('"CountryMaster"', 'CountryId'), 
       (SELECT MAX("CountryId") FROM "CountryMaster"));

-- =====================================================
-- 3. STATE MASTER (India + US + UK)
-- =====================================================
INSERT INTO "StateMaster" ("StateId", "CountryId", "StateName")
OVERRIDING SYSTEM VALUE
VALUES
    -- India (CountryId = 1)
    (1, 1, 'Andhra Pradesh'),
    (2, 1, 'Arunachal Pradesh'),
    (3, 1, 'Assam'),
    (4, 1, 'Bihar'),
    (5, 1, 'Chhattisgarh'),
    (6, 1, 'Goa'),
    (7, 1, 'Gujarat'),
    (8, 1, 'Haryana'),
    (9, 1, 'Himachal Pradesh'),
    (10, 1, 'Jharkhand'),
    (11, 1, 'Karnataka'),
    (12, 1, 'Kerala'),
    (13, 1, 'Madhya Pradesh'),
    (14, 1, 'Maharashtra'),
    (15, 1, 'Manipur'),
    (16, 1, 'Meghalaya'),
    (17, 1, 'Mizoram'),
    (18, 1, 'Nagaland'),
    (19, 1, 'Odisha'),
    (20, 1, 'Punjab'),
    (21, 1, 'Rajasthan'),
    (22, 1, 'Sikkim'),
    (23, 1, 'Tamil Nadu'),
    (24, 1, 'Telangana'),
    (25, 1, 'Tripura'),
    (26, 1, 'Uttar Pradesh'),
    (27, 1, 'Uttarakhand'),
    (28, 1, 'West Bengal'),
    (29, 1, 'Delhi'),
    (30, 1, 'Jammu and Kashmir'),
    (31, 1, 'Ladakh'),
    (32, 1, 'Chandigarh'),
    (33, 1, 'Puducherry'),
    -- United States (CountryId = 2)
    (34, 2, 'Alabama'),
    (35, 2, 'Alaska'),
    (36, 2, 'Arizona'),
    (37, 2, 'Arkansas'),
    (38, 2, 'California'),
    (39, 2, 'Colorado'),
    (40, 2, 'Connecticut'),
    (41, 2, 'Delaware'),
    (42, 2, 'Florida'),
    (43, 2, 'Georgia'),
    (44, 2, 'Hawaii'),
    (45, 2, 'Idaho'),
    (46, 2, 'Illinois'),
    (47, 2, 'Indiana'),
    (48, 2, 'Iowa'),
    (49, 2, 'Kansas'),
    (50, 2, 'Kentucky'),
    (51, 2, 'Louisiana'),
    (52, 2, 'Maine'),
    (53, 2, 'Maryland'),
    (54, 2, 'Massachusetts'),
    (55, 2, 'Michigan'),
    (56, 2, 'Minnesota'),
    (57, 2, 'Mississippi'),
    (58, 2, 'Missouri'),
    (59, 2, 'Montana'),
    (60, 2, 'Nebraska'),
    (61, 2, 'Nevada'),
    (62, 2, 'New Hampshire'),
    (63, 2, 'New Jersey'),
    (64, 2, 'New York'),
    (65, 2, 'North Carolina'),
    (66, 2, 'North Dakota'),
    (67, 2, 'Ohio'),
    (68, 2, 'Oklahoma'),
    (69, 2, 'Oregon'),
    (70, 2, 'Pennsylvania'),
    (71, 2, 'Rhode Island'),
    (72, 2, 'South Carolina'),
    (73, 2, 'South Dakota'),
    (74, 2, 'Tennessee'),
    (75, 2, 'Texas'),
    (76, 2, 'Utah'),
    (77, 2, 'Vermont'),
    (78, 2, 'Virginia'),
    (79, 2, 'Washington'),
    (80, 2, 'West Virginia'),
    (81, 2, 'Wisconsin'),
    (82, 2, 'Wyoming'),
    -- United Kingdom (CountryId = 3)
    (83, 3, 'England'),
    (84, 3, 'Scotland'),
    (85, 3, 'Wales'),
    (86, 3, 'Northern Ireland')
ON CONFLICT DO NOTHING;

SELECT setval(pg_get_serial_sequence('"StateMaster"', 'StateId'), 
       (SELECT MAX("StateId") FROM "StateMaster"));

-- =====================================================
-- 4. CITY MASTER (Major cities per state)
-- =====================================================
INSERT INTO "CityMaster" ("CityId", "StateId", "CityName")
OVERRIDING SYSTEM VALUE
VALUES
    -- Gujarat (StateId = 7)
    (1, 7, 'Ahmedabad'),
    (2, 7, 'Surat'),
    (3, 7, 'Vadodara'),
    (4, 7, 'Rajkot'),
    (5, 7, 'Gandhinagar'),
    (6, 7, 'Bhavnagar'),
    (7, 7, 'Jamnagar'),
    (8, 7, 'Junagadh'),
    (9, 7, 'Anand'),
    (10, 7, 'Morbi'),
    -- Maharashtra (StateId = 14)
    (11, 14, 'Mumbai'),
    (12, 14, 'Pune'),
    (13, 14, 'Nagpur'),
    (14, 14, 'Thane'),
    (15, 14, 'Nashik'),
    (16, 14, 'Aurangabad'),
    -- Karnataka (StateId = 11)
    (17, 11, 'Bengaluru'),
    (18, 11, 'Mysuru'),
    (19, 11, 'Mangaluru'),
    (20, 11, 'Hubli'),
    -- Tamil Nadu (StateId = 23)
    (21, 23, 'Chennai'),
    (22, 23, 'Coimbatore'),
    (23, 23, 'Madurai'),
    (24, 23, 'Salem'),
    -- Delhi (StateId = 29)
    (25, 29, 'New Delhi'),
    (26, 29, 'Central Delhi'),
    (27, 29, 'South Delhi'),
    -- Uttar Pradesh (StateId = 26)
    (28, 26, 'Lucknow'),
    (29, 26, 'Noida'),
    (30, 26, 'Agra'),
    (31, 26, 'Varanasi'),
    (32, 26, 'Kanpur'),
    -- Rajasthan (StateId = 21)
    (33, 21, 'Jaipur'),
    (34, 21, 'Udaipur'),
    (35, 21, 'Jodhpur'),
    -- Telangana (StateId = 24)
    (36, 24, 'Hyderabad'),
    (37, 24, 'Warangal'),
    -- West Bengal (StateId = 28)
    (38, 28, 'Kolkata'),
    (39, 28, 'Howrah'),
    -- Kerala (StateId = 12)
    (40, 12, 'Kochi'),
    (41, 12, 'Thiruvananthapuram'),
    -- Punjab (StateId = 20)
    (42, 20, 'Chandigarh'),
    (43, 20, 'Ludhiana'),
    (44, 20, 'Amritsar'),
    -- Haryana (StateId = 8)
    (45, 8, 'Gurugram'),
    (46, 8, 'Faridabad'),
    -- Andhra Pradesh (StateId = 1)
    (47, 1, 'Visakhapatnam'),
    (48, 1, 'Vijayawada'),
    (49, 1, 'Amaravati'),
    -- Madhya Pradesh (StateId = 13)
    (50, 13, 'Bhopal'),
    (51, 13, 'Indore'),
    -- Bihar (StateId = 4)
    (52, 4, 'Patna'),
    (53, 4, 'Gaya'),
    -- Goa (StateId = 6)
    (54, 6, 'Panaji'),
    (55, 6, 'Margao'),
    -- California (StateId = 38)
    (56, 38, 'Los Angeles'),
    (57, 38, 'San Francisco'),
    (58, 38, 'San Diego'),
    (59, 38, 'San Jose'),
    -- New York (StateId = 64)
    (60, 64, 'New York City'),
    (61, 64, 'Buffalo'),
    (62, 64, 'Albany'),
    -- Texas (StateId = 75)
    (63, 75, 'Houston'),
    (64, 75, 'Dallas'),
    (65, 75, 'Austin'),
    (66, 75, 'San Antonio'),
    -- Florida (StateId = 42)
    (67, 42, 'Miami'),
    (68, 42, 'Orlando'),
    (69, 42, 'Tampa'),
    -- Illinois (StateId = 46)
    (70, 46, 'Chicago'),
    -- Washington (StateId = 79)
    (71, 79, 'Seattle'),
    -- Georgia (StateId = 43)
    (72, 43, 'Atlanta'),
    -- Massachusetts (StateId = 54)
    (73, 54, 'Boston'),
    -- England (StateId = 83)
    (74, 83, 'London'),
    (75, 83, 'Manchester'),
    (76, 83, 'Birmingham'),
    (77, 83, 'Liverpool'),
    -- Scotland (StateId = 84)
    (78, 84, 'Edinburgh'),
    (79, 84, 'Glasgow')
ON CONFLICT DO NOTHING;

SELECT setval(pg_get_serial_sequence('"CityMaster"', 'CityId'), 
       (SELECT MAX("CityId") FROM "CityMaster"));

-- =====================================================
-- 5. DEFAULT ADMIN USER
-- =====================================================
-- Password: Admin@123 (BCrypt hash - update in your app)
-- NOTE: Your C# app generates PasswordHash + PasswordSalt.
--   This is a placeholder so you can login. Re-register or
--   update the hash via your app's registration flow.
INSERT INTO "UserMaster" ("UserName", "UserEmail", "PasswordHash", "PasswordSalt")
VALUES ('Admin', 'admin@demowebapp.com',
        'PLACEHOLDER_HASH_UPDATE_VIA_APP',
        'PLACEHOLDER_SALT_UPDATE_VIA_APP')
ON CONFLICT DO NOTHING;

-- =====================================================
-- 6. SAMPLE CATEGORIES
-- =====================================================
INSERT INTO "Categories" ("CategoryName", "ParentCategoryID", "CreatedBy")
VALUES
    ('Electronics', NULL, 1),
    ('Clothing', NULL, 1),
    ('Home & Kitchen', NULL, 1),
    ('Books', NULL, 1),
    ('Sports & Outdoors', NULL, 1),
    ('Mobile Phones', 1, 1),
    ('Laptops', 1, 1),
    ('Accessories', 1, 1),
    ('Men', 2, 1),
    ('Women', 2, 1),
    ('Kitchen Appliances', 3, 1),
    ('Furniture', 3, 1)
ON CONFLICT DO NOTHING;

-- =====================================================
-- 7. SAMPLE PRODUCTS + STOCK
-- =====================================================
INSERT INTO "Products" ("ProductName", "ProductDescription", "CategoryID", "Price", "SKU", "CreatedBy")
VALUES
    ('iPhone 15 Pro', '6.1-inch Super Retina XDR display, A17 Pro chip, 48MP camera', 6, 999.99, 'ELEC-MOB-001', 1),
    ('Samsung Galaxy S24', '6.2-inch Dynamic AMOLED, Snapdragon 8 Gen 3, 50MP camera', 6, 849.99, 'ELEC-MOB-002', 1),
    ('MacBook Air M3', '13.6-inch Liquid Retina, Apple M3 chip, 8GB RAM, 256GB SSD', 7, 1099.00, 'ELEC-LAP-001', 1),
    ('Dell XPS 15', '15.6-inch OLED, Intel i7-13700H, 16GB RAM, 512GB SSD', 7, 1299.00, 'ELEC-LAP-002', 1),
    ('AirPods Pro 2', 'Active Noise Cancellation, Adaptive Transparency, USB-C', 8, 249.00, 'ELEC-ACC-001', 1),
    ('Sony WH-1000XM5', 'Industry-leading noise cancellation, 30hr battery', 8, 349.99, 'ELEC-ACC-002', 1),
    ('Classic Cotton T-Shirt', '100% cotton, crew neck, available in multiple colors', 9, 29.99, 'CLO-MEN-001', 1),
    ('Slim Fit Denim Jeans', 'Stretch denim, 5-pocket design, dark wash', 9, 59.99, 'CLO-MEN-002', 1),
    ('Floral Summer Dress', 'Lightweight fabric, floral print, midi length', 10, 49.99, 'CLO-WOM-001', 1),
    ('High-Waist Leggings', 'Moisture-wicking fabric, 4-way stretch, squat-proof', 10, 39.99, 'CLO-WOM-002', 1),
    ('Instant Pot Duo 7-in-1', 'Pressure cooker, slow cooker, rice cooker, 6 quart', 11, 89.99, 'HOM-KIT-001', 1),
    ('KitchenAid Stand Mixer', 'Artisan Series, 5-quart, 10 speeds, tilt-head', 11, 379.99, 'HOM-KIT-002', 1),
    ('Ergonomic Office Chair', 'Mesh back, lumbar support, adjustable armrests', 12, 299.99, 'HOM-FUR-001', 1),
    ('Standing Desk', 'Electric height adjustable, 48x24 inch, memory presets', 12, 449.99, 'HOM-FUR-002', 1),
    ('The Pragmatic Programmer', 'Classic software development book by David Thomas & Andrew Hunt', 4, 49.95, 'BOK-TEC-001', 1),
    ('Yoga Mat Premium', 'Non-slip, 6mm thick, eco-friendly TPE material', 5, 34.99, 'SPT-YOG-001', 1),
    ('Running Shoes Pro', 'Lightweight mesh, responsive cushioning, all-terrain', 5, 129.99, 'SPT-RUN-001', 1),
    ('Resistance Bands Set', '5 levels, latex-free, with door anchor and carry bag', 5, 24.99, 'SPT-FIT-001', 1)
ON CONFLICT ("SKU") DO NOTHING;

-- Add stock for all products
INSERT INTO "Stock" ("ProductID", "Quantity", "ReorderLevel", "ReorderQuantity", "UpdatedBy")
SELECT "ProductID", 
       CASE 
           WHEN "Price" > 500 THEN 25 
           WHEN "Price" > 100 THEN 50 
           ELSE 100 
       END,
       10, 50, 1
FROM "Products"
WHERE NOT EXISTS (SELECT 1 FROM "Stock" S WHERE S."ProductID" = "Products"."ProductID");

-- =====================================================
-- 8. SAMPLE SUBSCRIPTION PLANS
-- =====================================================
INSERT INTO "Plans" ("PlanName", "PlanDescription", "StripeProductId")
VALUES
    ('Basic', 'Basic membership with 5% discount on all orders', 'prod_basic_placeholder'),
    ('Premium', 'Premium membership with 10% discount + free shipping', 'prod_premium_placeholder'),
    ('VIP', 'VIP membership with 15% discount + free shipping + priority support', 'prod_vip_placeholder')
ON CONFLICT DO NOTHING;

-- Plan Prices
INSERT INTO "PlanPrices" ("PlanId", "StripePriceId", "Amount", "Currency", "BillingInterval", "IntervalCount")
SELECT p."PlanId", 
       CASE p."PlanName"
           WHEN 'Basic' THEN 'price_basic_monthly_placeholder'
           WHEN 'Premium' THEN 'price_premium_monthly_placeholder'
           WHEN 'VIP' THEN 'price_vip_monthly_placeholder'
       END,
       CASE p."PlanName"
           WHEN 'Basic' THEN 4.99
           WHEN 'Premium' THEN 9.99
           WHEN 'VIP' THEN 19.99
       END,
       'usd', 'month', 1
FROM "Plans" p
WHERE NOT EXISTS (
    SELECT 1 FROM "PlanPrices" pp WHERE pp."PlanId" = p."PlanId"
);

-- Plan Benefits
INSERT INTO "PlanBenefits" ("PlanId", "BenefitKey", "BenefitValue")
SELECT p."PlanId", b."BenefitKey", b."BenefitValue"
FROM "Plans" p
CROSS JOIN (
    VALUES 
        ('Basic', 'discount_percent', '5'),
        ('Basic', 'free_shipping', 'false'),
        ('Basic', 'priority_support', 'false'),
        ('Premium', 'discount_percent', '10'),
        ('Premium', 'free_shipping', 'true'),
        ('Premium', 'priority_support', 'false'),
        ('VIP', 'discount_percent', '15'),
        ('VIP', 'free_shipping', 'true'),
        ('VIP', 'priority_support', 'true')
) AS b("PlanName", "BenefitKey", "BenefitValue")
WHERE p."PlanName" = b."PlanName"
  AND NOT EXISTS (
      SELECT 1 FROM "PlanBenefits" pb 
      WHERE pb."PlanId" = p."PlanId" AND pb."BenefitKey" = b."BenefitKey"
  );

-- =====================================================
-- 9. SAMPLE COUPONS
-- =====================================================
INSERT INTO "Coupons" ("CouponCode", "CouponName", "Description", "DiscountType", "DiscountValue",
    "StartDate", "EndDate", "MaxUsageCount", "MaxUsagePerUser", "CreatedBy")
VALUES
    ('WELCOME10', 'Welcome 10%', 'Get 10% off on your first order', 'Percentage', 10.00,
     CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '1 year', 1000, 1, 1),
    ('FLAT50', 'Flat $50 Off', 'Get flat $50 off on orders above $200', 'Flat', 50.00,
     CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '6 months', 500, 3, 1),
    ('SUMMER25', 'Summer Sale 25%', '25% discount during summer season', 'Percentage', 25.00,
     CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '3 months', 2000, 2, 1)
ON CONFLICT ("CouponCode") DO NOTHING;

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
DO $$
DECLARE
    _tbl RECORD;
    _count BIGINT;
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE ' SEED DATA VERIFICATION';
    RAISE NOTICE '========================================';
    
    FOR _tbl IN 
        SELECT unnest(ARRAY[
            'SitePolicies', 'CountryMaster', 'StateMaster', 'CityMaster',
            'UserMaster', 'Categories', 'Products', 'Stock',
            'Plans', 'PlanPrices', 'PlanBenefits', 'Coupons'
        ]) AS name
    LOOP
        EXECUTE format('SELECT COUNT(*) FROM %I', _tbl.name) INTO _count;
        RAISE NOTICE '  ✓ % : % rows', rpad(_tbl.name, 25), _count;
    END LOOP;
    
    RAISE NOTICE '========================================';
    RAISE NOTICE ' SEED DATA COMPLETE!';
    RAISE NOTICE '========================================';
END $$;


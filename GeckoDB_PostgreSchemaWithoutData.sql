-- =====================================================
-- DemoWebApp - PostgreSQL Migration Script
-- Target: neondb on GeckoServer (pgAdmin)
-- Converted from SQL Server to PostgreSQL
-- =====================================================
-- INSTRUCTIONS:
--   1. Open pgAdmin
--   2. Expand: GeckoServer → Databases → neondb
--   3. Right-click "neondb" → Query Tool
--   4. Paste this entire script
--   5. Press F5 (or click the Play ▶ button) to execute
-- =====================================================

-- Clean up if re-running (DROP in reverse dependency order)
-- Uncomment the next block if you need to re-run this script
/*
DROP FUNCTION IF EXISTS SP_ValidateUserToken(TEXT) CASCADE;
DROP FUNCTION IF EXISTS SP_ValidateCustomerToken(TEXT) CASCADE;
-- ... (add all functions)
DROP TABLE IF EXISTS "OrderStatusHistory" CASCADE;
DROP TABLE IF EXISTS "OrderDetail" CASCADE;
DROP TABLE IF EXISTS "Orders" CASCADE;
DROP TABLE IF EXISTS "StockTransaction" CASCADE;
DROP TABLE IF EXISTS "StockReservation" CASCADE;
DROP TABLE IF EXISTS "StockAlert" CASCADE;
DROP TABLE IF EXISTS "Stock" CASCADE;
DROP TABLE IF EXISTS "ProductCartItem" CASCADE;
DROP TABLE IF EXISTS "ProductCart" CASCADE;
DROP TABLE IF EXISTS "ProductImages" CASCADE;
DROP TABLE IF EXISTS "Products" CASCADE;
DROP TABLE IF EXISTS "CategoryImages" CASCADE;
DROP TABLE IF EXISTS "Categories" CASCADE;
DROP TABLE IF EXISTS "CustomerWishlist" CASCADE;
DROP TABLE IF EXISTS "CustomerWelcomeEmails" CASCADE;
DROP TABLE IF EXISTS "CustomerTokens" CASCADE;
DROP TABLE IF EXISTS "CustomerSubscriptions" CASCADE;
DROP TABLE IF EXISTS "CustomerAddress" CASCADE;
DROP TABLE IF EXISTS "Customers" CASCADE;
DROP TABLE IF EXISTS "CouponCustomers" CASCADE;
DROP TABLE IF EXISTS "Coupons" CASCADE;
DROP TABLE IF EXISTS "ContactUsRequests" CASCADE;
DROP TABLE IF EXISTS "CartNotificationLog" CASCADE;
DROP TABLE IF EXISTS "PlanBenefits" CASCADE;
DROP TABLE IF EXISTS "PlanPrices" CASCADE;
DROP TABLE IF EXISTS "Plans" CASCADE;
DROP TABLE IF EXISTS "StripeWebhookEvents" CASCADE;
DROP TABLE IF EXISTS "SitePolicies" CASCADE;
DROP TABLE IF EXISTS "CityMaster" CASCADE;
DROP TABLE IF EXISTS "StateMaster" CASCADE;
DROP TABLE IF EXISTS "CountryMaster" CASCADE;
DROP TABLE IF EXISTS "LoginAttemptLogs" CASCADE;
DROP TABLE IF EXISTS "UserTokens" CASCADE;
DROP TABLE IF EXISTS "UserMaster" CASCADE;
*/

-- =====================================================
-- TABLES
-- =====================================================

-- UserMaster
CREATE TABLE IF NOT EXISTS "UserMaster" (
    "UserId"       BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UserName"     TEXT NOT NULL,
    "UserEmail"    TEXT NOT NULL,
    "CreatedAt"    TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"    TIMESTAMP,
    "IsDeleted"    BOOLEAN DEFAULT FALSE,
    "DeletedAt"    TIMESTAMP,
    "Password"     TEXT,
    "PasswordHash" TEXT,
    "PasswordSalt" TEXT,
    "IsLocked"     BOOLEAN DEFAULT FALSE,
    "LockedTime"   TIMESTAMP
);

-- UserTokens
CREATE TABLE IF NOT EXISTS "UserTokens" (
    "UserTokenId"    BIGINT GENERATED ALWAYS AS IDENTITY,
    "UserId"         BIGINT NOT NULL,
    "JwtToken"       TEXT NOT NULL,
    "JwtCreatedDate" TIMESTAMP NOT NULL,
    "JwtExpiryDate"  TIMESTAMP NOT NULL
);

-- LoginAttemptLogs
CREATE TABLE IF NOT EXISTS "LoginAttemptLogs" (
    "AttemptId"   BIGINT GENERATED ALWAYS AS IDENTITY,
    "UserId"      BIGINT NOT NULL,
    "AttemptTime" TIMESTAMP,
    "IsSuccess"   BOOLEAN DEFAULT FALSE
);

-- CountryMaster
CREATE TABLE IF NOT EXISTS "CountryMaster" (
    "CountryId"   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CountryName" TEXT
);

-- StateMaster
CREATE TABLE IF NOT EXISTS "StateMaster" (
    "StateId"   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CountryId" BIGINT,
    "StateName" TEXT,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedBy" BIGINT
);

-- CityMaster
CREATE TABLE IF NOT EXISTS "CityMaster" (
    "CityId"    BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "StateId"   BIGINT,
    "CityName"  TEXT,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedBy" BIGINT
);

-- Categories
CREATE TABLE IF NOT EXISTS "Categories" (
    "CategoryID"       INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CategoryName"     VARCHAR(100) NOT NULL,
    "ParentCategoryID" INT,
    "CreatedAt"        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy"        BIGINT NOT NULL,
    "UpdatedAt"        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy"        BIGINT,
    "IsDeleted"        BOOLEAN DEFAULT FALSE,
    "DeletedBy"        BIGINT
);

-- CategoryImages
CREATE TABLE IF NOT EXISTS "CategoryImages" (
    "ImageID"    INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CategoryID" INT NOT NULL,
    "ImageUrl"   VARCHAR(500) NOT NULL,
    "IsPrimary"  BOOLEAN DEFAULT FALSE,
    "CreatedAt"  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy"  BIGINT NOT NULL,
    "IsDeleted"  BOOLEAN DEFAULT FALSE,
    "DeletedBy"  BIGINT
);

-- Products
CREATE TABLE IF NOT EXISTS "Products" (
    "ProductID"          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProductName"        VARCHAR(200) NOT NULL,
    "ProductDescription" TEXT,
    "CategoryID"         INT,
    "Price"              DECIMAL(10,2) NOT NULL,
    "SKU"                VARCHAR(50) NOT NULL UNIQUE,
    "CreatedAt"          TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"          TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy"          BIGINT NOT NULL,
    "UpdatedBy"          BIGINT,
    "IsDeleted"          BOOLEAN DEFAULT FALSE,
    "DeletedBy"          BIGINT
);

-- ProductImages
CREATE TABLE IF NOT EXISTS "ProductImages" (
    "ImageID"   INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProductID" INT NOT NULL,
    "ImageUrl"  VARCHAR(500) NOT NULL,
    "IsPrimary" BOOLEAN DEFAULT FALSE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" BIGINT NOT NULL,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedBy" BIGINT
);

-- Stock
CREATE TABLE IF NOT EXISTS "Stock" (
    "StockID"           INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProductID"         INT NOT NULL,
    "Quantity"          INT NOT NULL DEFAULT 0,
    "ReservedQuantity"  INT NOT NULL DEFAULT 0,
    "AvailableQuantity" INT GENERATED ALWAYS AS ("Quantity" - "ReservedQuantity") STORED,
    "ReorderLevel"      INT DEFAULT 10,
    "ReorderQuantity"   INT DEFAULT 50,
    "LastRestocked"     TIMESTAMP,
    "UpdatedAt"         TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy"         INT
);

-- StockAlert
CREATE TABLE IF NOT EXISTS "StockAlert" (
    "AlertID"           INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProductID"         INT NOT NULL,
    "AlertType"         VARCHAR(20),
    "CurrentQuantity"   INT,
    "ThresholdQuantity" INT,
    "AlertStatus"       VARCHAR(20) DEFAULT 'PENDING',
    "CreatedAt"         TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "AcknowledgedAt"    TIMESTAMP,
    "AcknowledgedBy"    INT
);

-- StockReservation
CREATE TABLE IF NOT EXISTS "StockReservation" (
    "ReservationID"   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProductID"       INT NOT NULL,
    "Quantity"        INT NOT NULL,
    "ReservationType" VARCHAR(20) NOT NULL,
    "ReferenceID"     BIGINT NOT NULL,
    "ExpiresAt"       TIMESTAMP NOT NULL,
    "Status"          VARCHAR(20) DEFAULT 'ACTIVE',
    "CreatedAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ReleasedAt"      TIMESTAMP
);

-- StockTransaction
CREATE TABLE IF NOT EXISTS "StockTransaction" (
    "TransactionID"    BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProductID"        INT NOT NULL,
    "TransactionType"  VARCHAR(20) NOT NULL,
    "Quantity"         INT NOT NULL,
    "ReferenceType"    VARCHAR(50),
    "ReferenceID"      BIGINT,
    "PreviousQuantity" INT,
    "NewQuantity"      INT,
    "PreviousReserved" INT,
    "NewReserved"      INT,
    "Reason"           VARCHAR(500),
    "CreatedAt"        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy"        INT
);

-- Customers
CREATE TABLE IF NOT EXISTS "Customers" (
    "CustomerId"        BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "FirstName"         TEXT NOT NULL,
    "LastName"          TEXT NOT NULL,
    "Email"             TEXT NOT NULL,
    "CountryCode"       VARCHAR(3) NOT NULL,
    "ContactNumber"     VARCHAR(15) NOT NULL,
    "CreatedAt"         TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy"         BIGINT,
    "UpdatedAt"         TIMESTAMP,
    "UpdatedBy"         BIGINT,
    "IsDeleted"         BOOLEAN DEFAULT FALSE,
    "DeletedAt"         TIMESTAMP,
    "DeletedBy"         BIGINT,
    "PasswordHash"      TEXT,
    "PasswordSalt"      TEXT,
    "CustomerPassword"  TEXT,
    "IsWelcomeMailSent" BOOLEAN DEFAULT FALSE,
    "FCMToken"          TEXT
);

-- CustomerTokens
CREATE TABLE IF NOT EXISTS "CustomerTokens" (
    "CustomerTokenId" BIGINT GENERATED ALWAYS AS IDENTITY,
    "CustomerId"      BIGINT NOT NULL,
    "JwtToken"        TEXT NOT NULL,
    "JwtCreatedDate"  TIMESTAMP NOT NULL,
    "JwtExpiryDate"   TIMESTAMP NOT NULL
);

-- CustomerAddress
CREATE TABLE IF NOT EXISTS "CustomerAddress" (
    "AddressId"   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "AddressName" TEXT NOT NULL,
    "FullAddress" TEXT NOT NULL,
    "CountryId"   BIGINT NOT NULL,
    "StateId"     BIGINT NOT NULL,
    "CityId"      BIGINT NOT NULL,
    "IsDefault"   BOOLEAN DEFAULT FALSE,
    "IsDeleted"   BOOLEAN DEFAULT FALSE,
    "UpdatedAt"   TIMESTAMP,
    "UpdatedBy"   BIGINT,
    "CustomerId"  BIGINT NOT NULL
);

-- CustomerWelcomeEmails
CREATE TABLE IF NOT EXISTS "CustomerWelcomeEmails" (
    "CustomerId"    BIGINT,
    "EmailTemplate" TEXT,
    "CreatedAt"     TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- CustomerWishlist
CREATE TABLE IF NOT EXISTS "CustomerWishlist" (
    "WishlistId" BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProductId"  BIGINT NOT NULL,
    "CustomerId" BIGINT NOT NULL,
    "CreatedAt"  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"  TIMESTAMP,
    "IsRemoved"  BOOLEAN DEFAULT FALSE
);

-- Plans
CREATE TABLE IF NOT EXISTS "Plans" (
    "PlanId"          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "PlanName"        VARCHAR(100) NOT NULL,
    "PlanDescription" VARCHAR(500),
    "StripeProductId" VARCHAR(100) NOT NULL,
    "IsActive"        BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt"       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"       TIMESTAMP
);

-- PlanPrices
CREATE TABLE IF NOT EXISTS "PlanPrices" (
    "PriceId"         INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "PlanId"          INT NOT NULL REFERENCES "Plans"("PlanId"),
    "StripePriceId"   VARCHAR(100) NOT NULL,
    "Amount"          DECIMAL(10,2) NOT NULL,
    "Currency"        VARCHAR(10) NOT NULL,
    "BillingInterval" VARCHAR(20) NOT NULL,
    "IntervalCount"   INT NOT NULL,
    "IsActive"        BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt"       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- PlanBenefits
CREATE TABLE IF NOT EXISTS "PlanBenefits" (
    "BenefitId"    INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "PlanId"       INT NOT NULL,
    "BenefitKey"   VARCHAR(100),
    "BenefitValue" VARCHAR(100)
);

-- CustomerSubscriptions
CREATE TABLE IF NOT EXISTS "CustomerSubscriptions" (
    "SubscriptionId"       INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CustomerId"           INT NOT NULL,
    "PlanId"               INT NOT NULL REFERENCES "Plans"("PlanId"),
    "StripeSubscriptionId" VARCHAR(100) NOT NULL,
    "SubscriptionStatus"   VARCHAR(50) NOT NULL,
    "CurrentPeriodStart"   TIMESTAMP,
    "CurrentPeriodEnd"     TIMESTAMP,
    "CancelAtPeriodEnd"    BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt"            TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"            TIMESTAMP
);

-- Coupons
CREATE TABLE IF NOT EXISTS "Coupons" (
    "CouponId"        BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CouponCode"      VARCHAR(50) NOT NULL UNIQUE,
    "CouponName"      VARCHAR(100),
    "Description"     VARCHAR(250),
    "DiscountType"    VARCHAR(20) NOT NULL CHECK ("DiscountType" IN ('Flat','Percentage')),
    "DiscountValue"   DECIMAL(10,2) NOT NULL,
    "StartDate"       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "EndDate"         TIMESTAMP NOT NULL,
    "MaxUsageCount"   INT,
    "UsageCount"      INT DEFAULT 0,
    "MaxUsagePerUser" INT,
    "IsActive"        BOOLEAN DEFAULT TRUE,
    "CreatedDate"     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy"       BIGINT,
    "IsDeleted"       BOOLEAN DEFAULT FALSE,
    "DeletedDate"     TIMESTAMP
);

-- CouponCustomers
CREATE TABLE IF NOT EXISTS "CouponCustomers" (
    "CouponCustomerId" BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CouponId"         BIGINT,
    "CustomerId"       BIGINT,
    "UsageCount"       INT DEFAULT 0,
    "LastUsedDate"     TIMESTAMP,
    "CartSessionId"    TEXT
);

-- ProductCart
CREATE TABLE IF NOT EXISTS "ProductCart" (
    "Id"             BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "SessionId"      VARCHAR(100) NOT NULL,
    "Token"          VARCHAR(100) NOT NULL UNIQUE,
    "Status"         SMALLINT NOT NULL DEFAULT 0,
    "Total"          DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    "CreatedAt"      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"      TIMESTAMP,
    "ExpiresAt"      TIMESTAMP NOT NULL DEFAULT (CURRENT_TIMESTAMP + INTERVAL '24 hours'),
    "CustomerId"     BIGINT,
    "CouponId"       BIGINT,
    "CouponCode"     VARCHAR(100),
    "DiscountType"   VARCHAR(50),
    "DiscountValue"  DECIMAL(10,2),
    "DiscountAmount" DECIMAL(10,2),
    "SubTotal"       DECIMAL(10,2) NOT NULL DEFAULT 0
);

-- ProductCartItem
CREATE TABLE IF NOT EXISTS "ProductCartItem" (
    "Id"            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CartSessionId" BIGINT NOT NULL,
    "ProductId"     BIGINT NOT NULL,
    "Price"         DECIMAL(10,2) NOT NULL,
    "Discount"      DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    "Quantity"      INT NOT NULL DEFAULT 1,
    "Active"        BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt"     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"     TIMESTAMP
);

-- Orders
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id"                    BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CartSessionId"         VARCHAR(255) NOT NULL,
    "OrderNumber"           VARCHAR(50) NOT NULL UNIQUE,
    "OrderStatus"           SMALLINT DEFAULT 1,
    "BillingAddress"        TEXT,
    "ShippingAddress"       TEXT,
    "ShippingSameAsBilling" BOOLEAN DEFAULT TRUE,
    "SubTotal"              DECIMAL(18,2) NOT NULL,
    "TaxAmount"             DECIMAL(18,2) DEFAULT 0.00,
    "ShippingAmount"        DECIMAL(18,2) DEFAULT 0.00,
    "DiscountAmount"        DECIMAL(18,2) DEFAULT 0.00,
    "Total"                 DECIMAL(18,2) NOT NULL,
    "PaymentMethod"         VARCHAR(100),
    "PaymentStatus"         SMALLINT DEFAULT 0,
    "PaymentTransactionId"  VARCHAR(255),
    "PaymentDate"           TIMESTAMP,
    "OrderNotes"            TEXT,
    "InternalNotes"         TEXT,
    "CreatedAt"             TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"             TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ShippedAt"             TIMESTAMP,
    "DeliveredAt"           TIMESTAMP,
    "IsActive"              BOOLEAN DEFAULT TRUE,
    "CustomerId"            BIGINT,
    "CouponCode"            VARCHAR(100),
    "PaymentIntentId"       TEXT,
    "StripePaymentStatus"   TEXT,
    "SubscriptionDiscount"  DECIMAL(18,4),
    "CurrentPlanId"         BIGINT
);

-- OrderDetail
CREATE TABLE IF NOT EXISTS "OrderDetail" (
    "Id"                 BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "OrderId"            BIGINT NOT NULL,
    "CartItemProductId"  BIGINT,
    "ProductId"          INT NOT NULL,
    "ProductName"        VARCHAR(255) NOT NULL,
    "ProductSKU"         VARCHAR(100),
    "ProductDescription" TEXT,
    "UnitPrice"          DECIMAL(18,2) NOT NULL,
    "Quantity"           INT NOT NULL,
    "Discount"           DECIMAL(18,2) DEFAULT 0.00,
    "LineTotal"          DECIMAL(18,2) GENERATED ALWAYS AS ("UnitPrice" * "Quantity" - "Discount") STORED,
    "CreatedAt"          TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- OrderStatusHistory
CREATE TABLE IF NOT EXISTS "OrderStatusHistory" (
    "Id"            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "OrderId"       BIGINT NOT NULL,
    "PreviousStatus" SMALLINT,
    "NewStatus"     SMALLINT NOT NULL,
    "StatusComment" VARCHAR(500),
    "ChangedBy"     INT,
    "CreatedAt"     TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ContactUsRequests
CREATE TABLE IF NOT EXISTS "ContactUsRequests" (
    "ContactUsId"     BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CustomerName"    TEXT,
    "CustomerEmail"   TEXT,
    "ContactSubject"  TEXT,
    "CustomerMessage" TEXT,
    "CreatedAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "IsReplied"       BOOLEAN DEFAULT FALSE,
    "IsRepliedAt"     TIMESTAMP,
    "AdminMessage"    TEXT,
    "IsDeleted"       BIGINT DEFAULT 0
);

-- SitePolicies
CREATE TABLE IF NOT EXISTS "SitePolicies" (
    "SitePolicyId"     BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "PolicyName"       VARCHAR(250),
    "PolicyDescription" TEXT,
    "CreatedDate"      TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedDate"      TIMESTAMP,
    "UpdatedBy"        BIGINT
);

-- CartNotificationLog
CREATE TABLE IF NOT EXISTS "CartNotificationLog" (
    "Id"               INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CartId"           INT,
    "CustomerId"       INT,
    "NotificationType" VARCHAR(250),
    "SentAt"           TIMESTAMP,
    "AttemptNumber"    INT
);

-- StripeWebhookEvents
CREATE TABLE IF NOT EXISTS "StripeWebhookEvents" (
    "Id"                BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "StripeEventId"     VARCHAR(255) NOT NULL,
    "EventType"         VARCHAR(100) NOT NULL,
    "ApiVersion"        VARCHAR(50),
    "PaymentIntentId"   VARCHAR(255),
    "CheckoutSessionId" VARCHAR(255),
    "ChargeId"          VARCHAR(255),
    "OrderNumber"       VARCHAR(100),
    "PayloadJson"       TEXT NOT NULL,
    "Processed"         BOOLEAN NOT NULL DEFAULT FALSE,
    "ProcessingError"   TEXT,
    "RetryCount"        INT NOT NULL DEFAULT 0,
    "CreatedAt"         TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ProcessedAt"       TIMESTAMP
);

-- =====================================================
-- INDEXES
-- =====================================================
CREATE INDEX IF NOT EXISTS "IX_Stock_Quantity" ON "Stock" ("ProductID", "Quantity") WHERE "Quantity" > 0;
CREATE INDEX IF NOT EXISTS "IX_Stock_Reserved" ON "Stock" ("ProductID", "ReservedQuantity") WHERE "ReservedQuantity" > 0;
CREATE INDEX IF NOT EXISTS "IX_StockRes_Expires" ON "StockReservation" ("Status", "ExpiresAt");
CREATE INDEX IF NOT EXISTS "IX_StockRes_Product" ON "StockReservation" ("ProductID", "Status");
CREATE INDEX IF NOT EXISTS "IX_StockTrans_Product" ON "StockTransaction" ("ProductID", "CreatedAt" DESC);
CREATE INDEX IF NOT EXISTS "IX_StockTrans_Reference" ON "StockTransaction" ("ReferenceType", "ReferenceID");


-- =====================================================
-- FUNCTIONS (PostgreSQL equivalent of Stored Procedures)
-- =====================================================
-- NOTE: In PostgreSQL, use FUNCTIONS that return tables/values.
-- Your C# code calls these via Npgsql with CommandType.StoredProcedure
-- which maps to: SELECT * FROM "function_name"(params);
-- =====================================================

-- =====================================================
-- HELPER / INTERNAL FUNCTIONS (called by other functions)
-- =====================================================

-- SP_UpdateCartTotal
CREATE OR REPLACE FUNCTION "SP_UpdateCartTotal"(_cartid BIGINT)
RETURNS VOID AS $$
BEGIN
    UPDATE "ProductCart"
    SET "SubTotal" = (
            SELECT COALESCE(SUM("Price" * "Quantity"), 0.00)
            FROM "ProductCartItem"
            WHERE "CartSessionId" = _cartid AND "Active" = TRUE
        ),
        "UpdatedAt" = NOW()
    WHERE "Id" = _cartid;
END;
$$ LANGUAGE plpgsql;

-- sp_RecalculateCartCoupon
CREATE OR REPLACE FUNCTION "sp_RecalculateCartCoupon"(_cartid BIGINT)
RETURNS VOID AS $$
DECLARE
    _couponid BIGINT;
    _discounttype VARCHAR(50);
    _discountvalue DECIMAL(10,2);
    _subtotal DECIMAL(10,2);
    _discountamount DECIMAL(10,2) := 0;
BEGIN
    SELECT "CouponId", "DiscountType", "DiscountValue", "SubTotal"
    INTO _couponid, _discounttype, _discountvalue, _subtotal
    FROM "ProductCart"
    WHERE "Id" = _cartid;

    IF _couponid IS NULL THEN
        UPDATE "ProductCart"
        SET "DiscountAmount" = 0, "Total" = "SubTotal"
        WHERE "Id" = _cartid;
        RETURN;
    END IF;

    IF _discounttype ILIKE '%PERCENT%' THEN
        _discountamount := (_subtotal * _discountvalue) / 100;
    ELSE
        _discountamount := _discountvalue;
    END IF;

    IF _discountamount > _subtotal THEN
        _discountamount := _subtotal;
    END IF;

    UPDATE "ProductCart"
    SET "DiscountAmount" = _discountamount,
        "Total" = "SubTotal" - _discountamount
    WHERE "Id" = _cartid;
END;
$$ LANGUAGE plpgsql;

-- SP_GetOrCreateCart
CREATE OR REPLACE FUNCTION "SP_GetOrCreateCart"(
    _sessionid VARCHAR(100) DEFAULT NULL,
    _customerid BIGINT DEFAULT NULL
)
RETURNS BIGINT AS $$
DECLARE
    _cartid BIGINT := NULL;
    _token VARCHAR(100);
    _newsessionid VARCHAR(100);
BEGIN
    IF _customerid IS NOT NULL THEN
        SELECT "Id" INTO _cartid
        FROM "ProductCart"
        WHERE "CustomerId" = _customerid AND "Status" = 0 AND "ExpiresAt" > NOW();
    ELSIF _sessionid IS NOT NULL THEN
        SELECT "Id" INTO _cartid
        FROM "ProductCart"
        WHERE "SessionId" = _sessionid AND "CustomerId" IS NULL AND "Status" = 0 AND "ExpiresAt" > NOW();
    END IF;

    IF _cartid IS NULL THEN
        _token := gen_random_uuid()::VARCHAR(100);
        IF _sessionid IS NULL THEN
            _newsessionid := 'cart_' || TO_CHAR(NOW(), 'YYYYMMDDHH24MISS') || '_' || LEFT(_token, 8);
        ELSE
            _newsessionid := _sessionid;
        END IF;

        INSERT INTO "ProductCart" ("SessionId", "CustomerId", "Token", "ExpiresAt", "CouponId", "CouponCode", "DiscountType", "DiscountValue", "DiscountAmount")
        VALUES (_newsessionid, _customerid, _token, NOW() + INTERVAL '24 hours', NULL, NULL, NULL, NULL, 0)
        RETURNING "Id" INTO _cartid;
    ELSE
        UPDATE "ProductCart"
        SET "ExpiresAt" = NOW() + INTERVAL '24 hours', "UpdatedAt" = NOW()
        WHERE "Id" = _cartid;
    END IF;

    RETURN _cartid;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- USER FUNCTIONS
-- =====================================================

-- SP_GetUsers
CREATE OR REPLACE FUNCTION "SP_GetUsers"()
RETURNS SETOF "UserMaster" AS $$
BEGIN
    RETURN QUERY SELECT * FROM "UserMaster" WHERE "IsDeleted" != TRUE;
END;
$$ LANGUAGE plpgsql;

-- SP_GetUserById
CREATE OR REPLACE FUNCTION "SP_GetUserById"(_userid BIGINT)
RETURNS SETOF "UserMaster" AS $$
BEGIN
    RETURN QUERY SELECT * FROM "UserMaster" WHERE "UserId" = _userid AND "IsDeleted" != TRUE;
END;
$$ LANGUAGE plpgsql;

-- SP_DeleteUser
CREATE OR REPLACE FUNCTION "SP_DeleteUser"(_userid BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "UserMaster" SET "IsDeleted" = TRUE WHERE "UserId" = _userid;
    RETURN QUERY SELECT 0;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveUser
CREATE OR REPLACE FUNCTION "SP_SaveUser"(
    _username TEXT,
    _useremail TEXT,
    _userid BIGINT,
    _passwordsalt TEXT,
    _passwordhash TEXT
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE
    _newid BIGINT;
BEGIN
    IF COALESCE(_userid, 0) = 0 THEN
        INSERT INTO "UserMaster"("UserName", "UserEmail", "PasswordHash", "PasswordSalt")
        VALUES(_username, _useremail, _passwordhash, _passwordsalt)
        RETURNING "UserId" INTO _newid;
        RETURN QUERY SELECT _newid;
    ELSE
        UPDATE "UserMaster"
        SET "UserName" = _username, "UserEmail" = _useremail, "UpdatedAt" = CURRENT_TIMESTAMP
        WHERE "UserId" = _userid;
        RETURN QUERY SELECT 0::BIGINT;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_GetUserByEmail
CREATE OR REPLACE FUNCTION "SP_GetUserByEmail"(_useremail TEXT)
RETURNS SETOF "UserMaster" AS $$
BEGIN
    IF EXISTS(SELECT 1 FROM "UserMaster" WHERE LOWER("UserEmail") = LOWER(_useremail) AND "IsDeleted" = FALSE) THEN
        RETURN QUERY SELECT * FROM "UserMaster" WHERE LOWER("UserEmail") = LOWER(_useremail) LIMIT 1;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- USER TOKEN FUNCTIONS
-- =====================================================

-- SP_InsertUserToken
CREATE OR REPLACE FUNCTION "SP_InsertUserToken"(
    _userid BIGINT,
    _jwttoken TEXT,
    _jwtcreateddate TIMESTAMP,
    _jwtexpirydate TIMESTAMP
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    INSERT INTO "UserTokens"("UserId","JwtToken","JwtCreatedDate","JwtExpiryDate")
    VALUES(_userid, _jwttoken, _jwtcreateddate, _jwtexpirydate);
    -- UserTokens has no PK identity easily, use currval
    _newid := currval(pg_get_serial_sequence('"UserTokens"', 'UserTokenId'));
    RETURN QUERY SELECT _newid;
END;
$$ LANGUAGE plpgsql;

-- SP_ValidateUserToken
CREATE OR REPLACE FUNCTION "SP_ValidateUserToken"(_jwttoken TEXT)
RETURNS TABLE("ValidationResult" INT) AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM "UserTokens"
        WHERE "JwtToken" = _jwttoken AND "JwtExpiryDate" > CURRENT_TIMESTAMP
    ) THEN
        RETURN QUERY SELECT 1;
    ELSE
        RETURN QUERY SELECT -1;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_GetUserDataByJWT
CREATE OR REPLACE FUNCTION "SP_GetUserDataByJWT"(_jwttoken TEXT)
RETURNS SETOF "UserTokens" AS $$
BEGIN
    IF EXISTS(SELECT 1 FROM "UserTokens" WHERE "JwtToken" LIKE '%' || _jwttoken || '%') THEN
        RETURN QUERY SELECT * FROM "UserTokens" WHERE "JwtToken" LIKE '%' || _jwttoken || '%';
    END IF;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- USER AUTH FUNCTIONS
-- =====================================================

-- SP_SaveLoginAttemptLogs
CREATE OR REPLACE FUNCTION "SP_SaveLoginAttemptLogs"(
    _userid BIGINT, _attempttime TIMESTAMP, _issuccess BOOLEAN
)
RETURNS TABLE(result INT) AS $$
BEGIN
    INSERT INTO "LoginAttemptLogs"("UserId","AttemptTime","IsSuccess")
    VALUES(_userid, _attempttime, _issuccess);
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_GetAttemptedLogs
CREATE OR REPLACE FUNCTION "SP_GetAttemptedLogs"(_userid BIGINT, _time TIMESTAMP)
RETURNS SETOF "LoginAttemptLogs" AS $$
BEGIN
    RETURN QUERY SELECT * FROM "LoginAttemptLogs"
    WHERE "UserId" = _userid
      AND "AttemptTime" BETWEEN (_time - INTERVAL '5 minutes') AND _time;
END;
$$ LANGUAGE plpgsql;

-- SP_ChangeLockStatus
CREATE OR REPLACE FUNCTION "SP_ChangeLockStatus"(
    _lockstatus BOOLEAN, _userid BIGINT, _lockedtime TIMESTAMP
)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "UserMaster" SET "IsLocked" = _lockstatus, "LockedTime" = _lockedtime
    WHERE "UserId" = _userid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;


-- =====================================================
-- CATEGORY FUNCTIONS
-- =====================================================

-- SP_SaveCategory
CREATE OR REPLACE FUNCTION "SP_SaveCategory"(
    _categoryid BIGINT, _parentcategoryid BIGINT DEFAULT NULL,
    _categoryname TEXT DEFAULT NULL, _createdby BIGINT DEFAULT NULL
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    IF _categoryid IS NOT NULL AND _categoryid > 0 THEN
        UPDATE "Categories" SET "ParentCategoryID" = _parentcategoryid,
            "CategoryName" = _categoryname, "UpdatedBy" = _createdby, "UpdatedAt" = NOW()
        WHERE "CategoryID" = _categoryid;
        RETURN QUERY SELECT 0::BIGINT;
    ELSE
        INSERT INTO "Categories"("ParentCategoryID","CategoryName","CreatedAt","CreatedBy")
        VALUES(_parentcategoryid, _categoryname, NOW(), _createdby)
        RETURNING "CategoryID" INTO _newid;
        RETURN QUERY SELECT _newid::BIGINT;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveCategoryImage
CREATE OR REPLACE FUNCTION "SP_SaveCategoryImage"(
    _categoryid BIGINT, _imageurl VARCHAR(500), _isprimary BOOLEAN, _createdby BIGINT
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    INSERT INTO "CategoryImages"("ImageUrl","IsPrimary","CreatedBy","CreatedAt","CategoryID")
    VALUES(_imageurl, _isprimary, _createdby, NOW(), _categoryid)
    RETURNING "ImageID" INTO _newid;
    RETURN QUERY SELECT _newid;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCategoryList
CREATE OR REPLACE FUNCTION "SP_GetCategoryList"()
RETURNS TABLE(
    "CategoryID" INT, "CategoryName" VARCHAR(100),
    "ImageUrl" VARCHAR(500), "ParentCategoryID" INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT C."CategoryID", C."CategoryName", CI."ImageUrl", C."ParentCategoryID"
    FROM "Categories" C
    LEFT JOIN "CategoryImages" CI ON C."CategoryID" = CI."CategoryID" AND CI."IsDeleted" != TRUE AND CI."IsPrimary" = TRUE
    WHERE C."IsDeleted" != TRUE;
END;
$$ LANGUAGE plpgsql;

-- SP_DeleteCategoryImage
CREATE OR REPLACE FUNCTION "SP_DeleteCategoryImage"(_imageids TEXT, _deletedby BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "CategoryImages" CI
    SET "IsDeleted" = TRUE, "DeletedBy" = _deletedby
    FROM unnest(string_to_array(_imageids, ',')) AS s(val)
    WHERE CI."ImageID" = TRIM(s.val)::INT;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCategoryImageById
CREATE OR REPLACE FUNCTION "SP_GetCategoryImageById"(_categoryid BIGINT)
RETURNS TABLE("ImageUrl" VARCHAR(500), "ImageID" INT) AS $$
BEGIN
    RETURN QUERY
    SELECT ci."ImageUrl", ci."ImageID"
    FROM "CategoryImages" ci
    WHERE ci."IsDeleted" = FALSE AND ci."CategoryID" = _categoryid;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- PRODUCT FUNCTIONS
-- =====================================================

-- SP_GetProductList (with pagination/sort/search)
CREATE OR REPLACE FUNCTION "SP_GetProductList"(
    _pagenumber INT DEFAULT 1, _pagesize INT DEFAULT 10,
    _searchterm VARCHAR(100) DEFAULT '', _sortcolumn VARCHAR(50) DEFAULT 'ProductName',
    _sortdirection VARCHAR(4) DEFAULT 'ASC'
)
RETURNS TABLE(
    "ProductID" INT, "ProductName" VARCHAR(200), "ProductDescription" TEXT,
    "Price" DECIMAL(10,2), "SKU" VARCHAR(50), "CategoryID" INT,
    "CategoryName" VARCHAR(100), "ProductImage" VARCHAR(500), "TotalRecords" BIGINT
) AS $$
DECLARE
    _offset INT;
    _searchvalue TEXT;
    _sql TEXT;
BEGIN
    IF _pagenumber < 1 THEN _pagenumber := 1; END IF;
    IF _pagesize < 1 THEN _pagesize := 10; END IF;
    IF _pagesize > 1000 THEN _pagesize := 1000; END IF;

    IF _sortcolumn NOT IN ('ProductID','ProductName','ProductDescription','Price','SKU','CategoryName','CategoryID') THEN
        _sortcolumn := 'ProductName';
    END IF;
    IF UPPER(_sortdirection) NOT IN ('ASC','DESC') THEN _sortdirection := 'ASC'; END IF;

    _offset := (_pagenumber - 1) * _pagesize;
    _searchvalue := '%' || TRIM(_searchterm) || '%';

    _sql := format(
        'WITH FilteredProducts AS (
            SELECT P."ProductID", P."ProductName", P."ProductDescription",
                   P."Price", P."SKU", P."CategoryID",
                   C."CategoryName", PI."ImageUrl" AS "ProductImage"
            FROM "Products" P
            INNER JOIN "Categories" C ON P."CategoryID" = C."CategoryID"
            LEFT JOIN "ProductImages" PI ON P."ProductID" = PI."ProductID" AND PI."IsPrimary" = TRUE AND PI."IsDeleted" = FALSE
            WHERE P."IsDeleted" = FALSE AND C."IsDeleted" = FALSE
              AND ($1 = '''' OR P."ProductName" ILIKE $2 OR P."ProductDescription" ILIKE $2 OR C."CategoryName" ILIKE $2)
        ),
        SortedProducts AS (
            SELECT *, ROW_NUMBER() OVER (ORDER BY %I %s) AS rn, COUNT(*) OVER() AS "TotalRecords"
            FROM FilteredProducts
        )
        SELECT "ProductID","ProductName","ProductDescription","Price","SKU","CategoryID","CategoryName","ProductImage","TotalRecords"
        FROM SortedProducts WHERE rn BETWEEN ($3 + 1) AND ($3 + $4) ORDER BY rn',
        _sortcolumn, _sortdirection
    );

    RETURN QUERY EXECUTE _sql USING _searchterm, _searchvalue, _offset, _pagesize;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveProducts
CREATE OR REPLACE FUNCTION "SP_SaveProducts"(
    _productid BIGINT, _categoryid BIGINT, _productname TEXT,
    _productdescription TEXT, _price DECIMAL(10,2), _sku VARCHAR(50), _createdby BIGINT
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    IF _productid IS NOT NULL AND _productid > 0 THEN
        UPDATE "Products" SET "ProductName" = _productname, "ProductDescription" = _productdescription,
            "SKU" = _sku, "UpdatedBy" = _createdby, "UpdatedAt" = NOW(), "Price" = _price
        WHERE "ProductID" = _productid;
        RETURN QUERY SELECT 0::BIGINT;
    ELSE
        INSERT INTO "Products"("ProductName","ProductDescription","SKU","CreatedBy","CategoryID","CreatedAt","Price")
        VALUES(_productname, _productdescription, _sku, _createdby, _categoryid, NOW(), _price)
        RETURNING "ProductID" INTO _newid;
        RETURN QUERY SELECT _newid::BIGINT;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveProductImage
CREATE OR REPLACE FUNCTION "SP_SaveProductImage"(
    _productid BIGINT, _imageurl VARCHAR(500), _isprimary BOOLEAN, _createdby BIGINT
)
RETURNS TABLE("ImageId" BIGINT) AS $$
DECLARE _latestid BIGINT;
BEGIN
    INSERT INTO "ProductImages"("ImageUrl","IsPrimary","CreatedBy","CreatedAt","ProductID")
    VALUES(_imageurl, _isprimary, _createdby, NOW(), _productid)
    RETURNING "ImageID" INTO _latestid;

    IF NOT EXISTS (SELECT 1 FROM "ProductImages" WHERE "ProductID" = _productid AND "IsPrimary" = TRUE) THEN
        UPDATE "ProductImages" SET "IsPrimary" = TRUE WHERE "ImageID" = _latestid;
    END IF;
    RETURN QUERY SELECT _latestid;
END;
$$ LANGUAGE plpgsql;

-- SP_GetProductListByCategoryId
CREATE OR REPLACE FUNCTION "SP_GetProductListByCategoryId"(_categoryid BIGINT DEFAULT 0)
RETURNS TABLE(
    "ProductID" INT, "ProductName" VARCHAR(200), "Price" DECIMAL(10,2),
    "ProductDescription" TEXT, "ProductImage" VARCHAR(500), "CategoryID" INT,
    "CategoryName" VARCHAR(100), "SKU" VARCHAR(50), "CreatedAt" TIMESTAMP, "UpdatedAt" TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    SELECT P."ProductID", P."ProductName", P."Price", P."ProductDescription",
           PIM."ImageUrl", P."CategoryID", C."CategoryName", P."SKU", P."CreatedAt", P."UpdatedAt"
    FROM "Products" P
    LEFT JOIN "ProductImages" PIM ON P."ProductID" = PIM."ProductID" AND PIM."IsPrimary" = TRUE
    LEFT JOIN "Categories" C ON C."CategoryID" = P."CategoryID"
    WHERE (_categoryid = 0 OR (P."CategoryID" = _categoryid OR C."ParentCategoryID" = _categoryid))
      AND (P."IsDeleted" = FALSE OR P."IsDeleted" IS NULL)
      AND (C."IsDeleted" = FALSE OR C."IsDeleted" IS NULL)
    ORDER BY P."ProductName";
END;
$$ LANGUAGE plpgsql;

-- SP_GetProductListByCategoryId_InStockOnly
CREATE OR REPLACE FUNCTION "SP_GetProductListByCategoryId_InStockOnly"(
    _categoryid BIGINT DEFAULT 0, _customerid BIGINT DEFAULT 0
)
RETURNS TABLE(
    "ProductID" INT, "ProductName" VARCHAR(200), "Price" DECIMAL(10,2),
    "ProductDescription" TEXT, "ProductImage" VARCHAR(500), "CategoryID" INT,
    "CategoryName" VARCHAR(100), "SKU" VARCHAR(50), "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP, "AvailableQuantity" INT, "IsLowStock" INT, "IsWishlistItem" BOOLEAN
) AS $$
BEGIN
    RETURN QUERY
    SELECT P."ProductID", P."ProductName", P."Price", P."ProductDescription",
           PIM."ImageUrl", P."CategoryID", C."CategoryName", P."SKU", P."CreatedAt", P."UpdatedAt",
           COALESCE(S."AvailableQuantity", 0),
           CASE WHEN S."AvailableQuantity" <= S."ReorderLevel" THEN 1 ELSE 0 END,
           CASE WHEN CW."ProductId" IS NOT NULL THEN TRUE ELSE FALSE END
    FROM "Products" P
    LEFT JOIN "CustomerWishlist" CW ON P."ProductID" = CW."ProductId" AND CW."CustomerId" = _customerid AND CW."IsRemoved" = FALSE
    LEFT JOIN "ProductImages" PIM ON P."ProductID" = PIM."ProductID" AND PIM."IsPrimary" = TRUE
    LEFT JOIN "Categories" C ON C."CategoryID" = P."CategoryID"
    INNER JOIN "Stock" S ON S."ProductID" = P."ProductID"
    WHERE (_categoryid = 0 OR (P."CategoryID" = _categoryid OR C."ParentCategoryID" = _categoryid))
      AND (P."IsDeleted" = FALSE OR P."IsDeleted" IS NULL)
      AND (C."IsDeleted" = FALSE OR C."IsDeleted" IS NULL)
      AND S."AvailableQuantity" > 0
    ORDER BY P."ProductName";
END;
$$ LANGUAGE plpgsql;

-- SP_GetProductImages
CREATE OR REPLACE FUNCTION "SP_GetProductImages"(_productid BIGINT)
RETURNS TABLE("ImageID" INT, "ProductID" INT, "ImageUrl" VARCHAR(500), "IsPrimary" BOOLEAN) AS $$
BEGIN
    RETURN QUERY SELECT pi."ImageID", pi."ProductID", pi."ImageUrl", pi."IsPrimary"
    FROM "ProductImages" pi WHERE pi."ProductID" = _productid AND pi."IsDeleted" = FALSE;
END;
$$ LANGUAGE plpgsql;

-- SP_DeleteProductImages
CREATE OR REPLACE FUNCTION "SP_DeleteProductImages"(_imageid BIGINT, _deletedby BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "ProductImages" SET "IsDeleted" = TRUE, "DeletedBy" = _deletedby WHERE "ImageID" = _imageid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_SetPrimaryProductImage
CREATE OR REPLACE FUNCTION "SP_SetPrimaryProductImage"(_imageid BIGINT)
RETURNS TABLE(result INT) AS $$
DECLARE _productid BIGINT;
BEGIN
    SELECT "ProductID" INTO _productid FROM "ProductImages" WHERE "ImageID" = _imageid;
    UPDATE "ProductImages" SET "IsPrimary" = FALSE WHERE "ProductID" = _productid;
    UPDATE "ProductImages" SET "IsPrimary" = TRUE WHERE "ImageID" = _imageid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;


-- =====================================================
-- PRODUCT STOCK FUNCTIONS
-- =====================================================

-- SP_GetProductStockDetails
CREATE OR REPLACE FUNCTION "SP_GetProductStockDetails"(_productid BIGINT)
RETURNS TABLE("ProductID" INT, "StockID" INT, "Quantity" INT) AS $$
BEGIN
    RETURN QUERY SELECT s."ProductID", s."StockID", s."Quantity"
    FROM "Stock" s WHERE s."ProductID" = _productid;
END;
$$ LANGUAGE plpgsql;

-- SP_UpdateProductStock
CREATE OR REPLACE FUNCTION "SP_UpdateProductStock"(
    _stockid BIGINT, _productid BIGINT, _quantity BIGINT, _updatedby BIGINT
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    IF _quantity IS NULL OR _quantity < 0 THEN
        RETURN QUERY SELECT -1::BIGINT; RETURN;
    END IF;

    IF EXISTS (SELECT 1 FROM "Stock" WHERE "ProductID" = _productid AND (_stockid IS NULL OR _stockid = 0)) THEN
        RETURN QUERY SELECT -2::BIGINT; RETURN;
    END IF;

    IF _stockid IS NOT NULL AND _stockid > 0 THEN
        UPDATE "Stock" SET "Quantity" = _quantity, "UpdatedAt" = CURRENT_TIMESTAMP, "UpdatedBy" = _updatedby
        WHERE "StockID" = _stockid AND "ProductID" = _productid;
        RETURN QUERY SELECT 0::BIGINT;
    ELSE
        INSERT INTO "Stock"("ProductID","Quantity","UpdatedAt","UpdatedBy")
        VALUES(_productid, _quantity, CURRENT_TIMESTAMP, _updatedby)
        RETURNING "StockID" INTO _newid;
        RETURN QUERY SELECT _newid::BIGINT;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_ReleaseExpiredReservations
CREATE OR REPLACE FUNCTION "SP_ReleaseExpiredReservations"()
RETURNS TABLE("ReleasedCount" BIGINT) AS $$
DECLARE
    _expired RECORD;
    _count BIGINT := 0;
BEGIN
    FOR _expired IN
        SELECT "ReservationID", "ProductID", "Quantity"
        FROM "StockReservation"
        WHERE "Status" = 'ACTIVE' AND "ExpiresAt" < CURRENT_TIMESTAMP
        FOR UPDATE
    LOOP
        UPDATE "Stock" SET "ReservedQuantity" = "ReservedQuantity" - _expired."Quantity"
        WHERE "ProductID" = _expired."ProductID";

        UPDATE "StockReservation" SET "Status" = 'RELEASED', "ReleasedAt" = CURRENT_TIMESTAMP
        WHERE "ReservationID" = _expired."ReservationID";

        INSERT INTO "StockTransaction"("ProductID","TransactionType","Quantity","ReferenceType","ReferenceID","Reason")
        VALUES(_expired."ProductID", 'RELEASE', _expired."Quantity", 'RESERVATION_EXPIRED', _expired."ReservationID", 'Auto-released expired reservation');

        _count := _count + 1;
    END LOOP;
    RETURN QUERY SELECT _count;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- CART FUNCTIONS
-- =====================================================

-- SP_AddToCart
CREATE OR REPLACE FUNCTION "SP_AddToCart"(
    _sessionid VARCHAR(100) DEFAULT NULL, _customerid BIGINT DEFAULT NULL,
    _productid BIGINT DEFAULT NULL, _price DECIMAL(10,2) DEFAULT NULL, _quantity INT DEFAULT 1
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE
    _cartid BIGINT;
    _existingqty INT := 0;
BEGIN
    _cartid := "SP_GetOrCreateCart"(_sessionid, _customerid);

    SELECT "Quantity" INTO _existingqty
    FROM "ProductCartItem"
    WHERE "CartSessionId" = _cartid AND "ProductId" = _productid AND "Active" = TRUE;

    IF _existingqty IS NOT NULL AND _existingqty > 0 THEN
        UPDATE "ProductCartItem"
        SET "Quantity" = "Quantity" + _quantity, "Price" = _price, "UpdatedAt" = NOW()
        WHERE "CartSessionId" = _cartid AND "ProductId" = _productid AND "Active" = TRUE;
    ELSE
        INSERT INTO "ProductCartItem"("CartSessionId","ProductId","Price","Quantity")
        VALUES(_cartid, _productid, _price, _quantity);
    END IF;

    PERFORM "SP_UpdateCartTotal"(_cartid);
    PERFORM "sp_RecalculateCartCoupon"(_cartid);

    RETURN QUERY SELECT COUNT(DISTINCT "ProductId")::BIGINT
    FROM "ProductCartItem" WHERE "CartSessionId" = _cartid AND "Active" = TRUE;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT 0::BIGINT;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCartContents
CREATE OR REPLACE FUNCTION "SP_GetCartContents"(
    _sessionid VARCHAR(100) DEFAULT NULL, _customerid BIGINT DEFAULT NULL
)
RETURNS TABLE(
    "ItemId" BIGINT, "ProductId" BIGINT, "ProductName" VARCHAR(200),
    "ProductDescription" TEXT, "ImageUrl" VARCHAR(500), "Price" DECIMAL(10,2),
    "Discount" DECIMAL(10,2), "Quantity" INT, "CreatedAt" TIMESTAMP,
    "ItemTotal" DECIMAL, "CartId" BIGINT, "CustomerId" BIGINT,
    "CouponId" BIGINT, "CouponCode" VARCHAR(100), "DiscountType" VARCHAR(50),
    "DiscountValue" DECIMAL(10,2), "DiscountAmount" DECIMAL(10,2), "SubTotal" DECIMAL(10,2),
    "StockQuantity" INT, "ReservedQuantity" INT, "AvailableQuantity" INT,
    "StockStatus" TEXT, "MaxAvailableQuantity" INT, "IsQuantityAvailable" INT,
    "TaxPercentage" TEXT, "SubscriptionDiscount" VARCHAR(100),
    "IsFreeShipping" VARCHAR(100), "CurrentPlanName" VARCHAR(100)
) AS $$
DECLARE _cartid BIGINT;
BEGIN
    IF _sessionid IS NOT NULL THEN
        SELECT pc."Id" INTO _cartid FROM "ProductCart" pc
        WHERE pc."SessionId" = _sessionid AND pc."Status" = 0 AND pc."ExpiresAt" > NOW();
    ELSIF _customerid IS NOT NULL THEN
        SELECT pc."Id" INTO _cartid FROM "ProductCart" pc
        WHERE pc."CustomerId" = _customerid AND pc."Status" = 0 AND pc."ExpiresAt" > NOW();
    END IF;

    RETURN QUERY
    SELECT
        pci."Id", pci."ProductId", p."ProductName", p."ProductDescription",
        pim."ImageUrl", pci."Price", pci."Discount", pci."Quantity", pci."CreatedAt",
        (pci."Price" - pci."Discount") * pci."Quantity",
        _cartid, pc2."CustomerId",
        pc2."CouponId", pc2."CouponCode", pc2."DiscountType",
        pc2."DiscountValue", pc2."DiscountAmount", pc2."SubTotal",
        COALESCE(s."Quantity", 0), COALESCE(s."ReservedQuantity", 0), COALESCE(s."AvailableQuantity", 0),
        CASE
            WHEN s."ProductID" IS NULL THEN 'NOT_TRACKED'
            WHEN s."AvailableQuantity" = 0 THEN 'OUT_OF_STOCK'
            WHEN s."AvailableQuantity" < pci."Quantity" THEN 'INSUFFICIENT_STOCK'
            WHEN s."AvailableQuantity" <= s."ReorderLevel" THEN 'LOW_STOCK'
            ELSE 'IN_STOCK'
        END,
        CASE
            WHEN s."ProductID" IS NULL THEN pci."Quantity"
            WHEN s."AvailableQuantity" < pci."Quantity" THEN s."AvailableQuantity"
            ELSE pci."Quantity"
        END,
        CASE
            WHEN s."ProductID" IS NULL THEN 1
            WHEN s."AvailableQuantity" >= pci."Quantity" THEN 1
            ELSE 0
        END,
        (SELECT sp."PolicyDescription" FROM "SitePolicies" sp WHERE sp."PolicyName" LIKE '%TaxPercentage%'),
        (SELECT pb."BenefitValue" FROM "CustomerSubscriptions" cs2
         INNER JOIN "Plans" pl ON cs2."PlanId" = pl."PlanId"
         INNER JOIN "PlanBenefits" pb ON pl."PlanId" = pb."PlanId"
         WHERE pb."BenefitKey" = 'discount_percent' AND cs2."CustomerId" = _customerid AND cs2."SubscriptionStatus" = 'active'),
        (SELECT pb."BenefitValue" FROM "CustomerSubscriptions" cs2
         INNER JOIN "Plans" pl ON cs2."PlanId" = pl."PlanId"
         INNER JOIN "PlanBenefits" pb ON pl."PlanId" = pb."PlanId"
         WHERE pb."BenefitKey" = 'free_shipping' AND cs2."CustomerId" = _customerid AND cs2."SubscriptionStatus" = 'active'),
        (SELECT pl."PlanName" FROM "CustomerSubscriptions" cs2
         INNER JOIN "Plans" pl ON cs2."PlanId" = pl."PlanId"
         WHERE cs2."CustomerId" = _customerid AND cs2."SubscriptionStatus" = 'active')
    FROM "ProductCartItem" pci
    INNER JOIN "Products" p ON p."ProductID" = pci."ProductId"
    LEFT JOIN "ProductImages" pim ON pim."ProductID" = p."ProductID" AND pim."IsPrimary" = TRUE
    INNER JOIN "ProductCart" pc2 ON pc2."Id" = pci."CartSessionId"
    LEFT JOIN "Stock" s ON s."ProductID" = pci."ProductId"
    WHERE pci."CartSessionId" = _cartid AND pci."Active" = TRUE
    ORDER BY pci."CreatedAt" DESC;
END;
$$ LANGUAGE plpgsql;

-- SP_UpdateCartItemQuantity
CREATE OR REPLACE FUNCTION "SP_UpdateCartItemQuantity"(
    _sessionid VARCHAR(100) DEFAULT NULL, _customerid BIGINT DEFAULT NULL,
    _productid BIGINT DEFAULT NULL, _newquantity INT DEFAULT NULL
)
RETURNS TABLE(result INT) AS $$
DECLARE _cartid BIGINT;
BEGIN
    IF _customerid IS NOT NULL THEN
        SELECT "Id" INTO _cartid FROM "ProductCart"
        WHERE "CustomerId" = _customerid AND "Status" = 0 AND "ExpiresAt" > NOW();
    ELSE
        SELECT "Id" INTO _cartid FROM "ProductCart"
        WHERE "SessionId" = _sessionid AND "CustomerId" IS NULL AND "Status" = 0 AND "ExpiresAt" > NOW();
    END IF;

    IF _cartid IS NULL THEN
        RETURN QUERY SELECT -1; RETURN;
    END IF;

    IF _newquantity <= 0 THEN
        UPDATE "ProductCartItem" SET "Active" = FALSE, "UpdatedAt" = NOW()
        WHERE "CartSessionId" = _cartid AND "ProductId" = _productid;
    ELSE
        UPDATE "ProductCartItem" SET "Quantity" = _newquantity, "UpdatedAt" = NOW()
        WHERE "CartSessionId" = _cartid AND "ProductId" = _productid AND "Active" = TRUE;
    END IF;

    PERFORM "SP_UpdateCartTotal"(_cartid);
    PERFORM "sp_RecalculateCartCoupon"(_cartid);
    RETURN QUERY SELECT 1;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT -2;
END;
$$ LANGUAGE plpgsql;

-- SP_UpdateCartCustomerId
CREATE OR REPLACE FUNCTION "SP_UpdateCartCustomerId"(_customerid BIGINT, _cartid BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    IF CURRENT_TIMESTAMP <= (SELECT "ExpiresAt" FROM "ProductCart" WHERE "Id" = _cartid) THEN
        UPDATE "ProductCart" SET "CustomerId" = _customerid WHERE "Id" = _cartid;
        RETURN QUERY SELECT 1;
    ELSE
        RETURN QUERY SELECT -1;
    END IF;
END;
$$ LANGUAGE plpgsql;


-- =====================================================
-- COUPON FUNCTIONS
-- =====================================================

-- SP_ApplyCoupon
CREATE OR REPLACE FUNCTION "SP_ApplyCoupon"(_cartsessionid VARCHAR(100), _couponcode VARCHAR(100))
RETURNS TABLE("StatusCode" INT, "Message" TEXT) AS $$
DECLARE
    _couponid BIGINT; _isactive BOOLEAN; _startdate TIMESTAMP; _enddate TIMESTAMP;
    _maxusagecount INT; _usagecount INT; _discounttype VARCHAR(50); _discountvalue DECIMAL(10,2);
    _cartid BIGINT; _customerid BIGINT; _usagepercustomer INT; _usedcount INT := 0;
BEGIN
    SELECT "Id" INTO _cartid FROM "ProductCart" WHERE "SessionId" = _cartsessionid AND "Status" = 0;
    IF _cartid IS NULL THEN RETURN QUERY SELECT -10, 'Cart not found.'::TEXT; RETURN; END IF;

    SELECT c."CouponId",c."IsActive",c."StartDate",c."EndDate",c."MaxUsageCount",c."UsageCount",c."DiscountType",c."DiscountValue"
    INTO _couponid,_isactive,_startdate,_enddate,_maxusagecount,_usagecount,_discounttype,_discountvalue
    FROM "Coupons" c WHERE c."CouponCode" = _couponcode;

    IF _couponid IS NULL THEN RETURN QUERY SELECT -1, 'Coupon does not exist.'::TEXT; RETURN; END IF;
    IF _isactive = FALSE THEN RETURN QUERY SELECT -2, 'Coupon is inactive.'::TEXT; RETURN; END IF;
    IF CURRENT_TIMESTAMP < _startdate THEN RETURN QUERY SELECT -3, 'Coupon is not active yet.'::TEXT; RETURN; END IF;
    IF CURRENT_TIMESTAMP > _enddate THEN RETURN QUERY SELECT -4, 'Coupon has expired.'::TEXT; RETURN; END IF;
    IF _maxusagecount IS NOT NULL AND _usagecount >= _maxusagecount THEN RETURN QUERY SELECT -5, 'Coupon usage limit reached.'::TEXT; RETURN; END IF;

    SELECT pc."CustomerId" INTO _customerid FROM "ProductCart" pc WHERE pc."SessionId" = _cartsessionid;
    IF _customerid IS NULL THEN RETURN; END IF;

    SELECT c."MaxUsagePerUser" INTO _usagepercustomer FROM "Coupons" c WHERE c."CouponId" = _couponid;
    IF _usagepercustomer IS NOT NULL THEN
        SELECT COALESCE(cc."UsageCount", 0) INTO _usedcount FROM "CouponCustomers" cc WHERE cc."CustomerId" = _customerid AND cc."CouponId" = _couponid;
        _usedcount := COALESCE(_usedcount, 0);
        IF _usedcount >= _usagepercustomer THEN
            RETURN QUERY SELECT -6, 'You have already used this coupon the maximum allowed times.'::TEXT; RETURN;
        END IF;
    END IF;

    UPDATE "ProductCart" SET "CouponId" = _couponid, "CouponCode" = _couponcode,
        "DiscountType" = _discounttype, "DiscountValue" = _discountvalue, "UpdatedAt" = CURRENT_TIMESTAMP
    WHERE "Id" = _cartid;

    PERFORM "sp_RecalculateCartCoupon"(_cartid);
    RETURN QUERY SELECT 1, 'Coupon applied successfully'::TEXT;
END;
$$ LANGUAGE plpgsql;

-- SP_RemoveCoupon
CREATE OR REPLACE FUNCTION "SP_RemoveCoupon"(_cartsessionid VARCHAR(100))
RETURNS TABLE(result INT) AS $$
DECLARE _cartid BIGINT;
BEGIN
    SELECT "Id" INTO _cartid FROM "ProductCart" WHERE "SessionId" = _cartsessionid AND "Status" = 0;
    IF _cartid IS NULL THEN RETURN QUERY SELECT -1; RETURN; END IF;

    UPDATE "ProductCart" SET "CouponId" = NULL, "CouponCode" = NULL, "DiscountType" = NULL,
        "DiscountValue" = NULL, "DiscountAmount" = 0, "Total" = "SubTotal", "UpdatedAt" = CURRENT_TIMESTAMP
    WHERE "Id" = _cartid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCouponList
CREATE OR REPLACE FUNCTION "SP_GetCouponList"(
    _pagenumber INT DEFAULT 1, _pagesize INT DEFAULT 10,
    _searchterm VARCHAR(100) DEFAULT '', _sortcolumn VARCHAR(50) DEFAULT 'CouponCode',
    _sortdirection VARCHAR(4) DEFAULT 'ASC'
)
RETURNS TABLE(
    "CouponId" BIGINT, "CouponCode" VARCHAR(50), "CouponName" VARCHAR(100),
    "Description" VARCHAR(250), "DiscountType" VARCHAR(20), "DiscountValue" DECIMAL(10,2),
    "StartDate" TIMESTAMP, "EndDate" TIMESTAMP, "MaxUsageCount" INT, "UsageCount" INT,
    "MaxUsagePerUser" INT, "IsActive" BOOLEAN, "CreatedDate" TIMESTAMP, "TotalRecords" BIGINT
) AS $$
DECLARE _offset INT; _searchvalue TEXT; _sql TEXT;
BEGIN
    IF _pagenumber < 1 THEN _pagenumber := 1; END IF;
    IF _pagesize < 1 THEN _pagesize := 10; END IF;
    IF _pagesize > 1000 THEN _pagesize := 1000; END IF;
    IF _sortcolumn NOT IN ('CouponId','CouponCode','CouponName','DiscountType','DiscountValue','StartDate','EndDate','IsActive') THEN _sortcolumn := 'CouponCode'; END IF;
    IF UPPER(_sortdirection) NOT IN ('ASC','DESC') THEN _sortdirection := 'ASC'; END IF;
    _offset := (_pagenumber - 1) * _pagesize;
    _searchvalue := '%' || TRIM(_searchterm) || '%';

    _sql := format(
        'WITH Filtered AS (
            SELECT "CouponId","CouponCode","CouponName","Description","DiscountType","DiscountValue",
                   "StartDate","EndDate","MaxUsageCount","UsageCount","MaxUsagePerUser","IsActive","CreatedDate"
            FROM "Coupons"
            WHERE ($1 = '''' OR "CouponCode" ILIKE $2 OR "CouponName" ILIKE $2 OR "Description" ILIKE $2)
        ),
        Sorted AS (
            SELECT *, ROW_NUMBER() OVER (ORDER BY %I %s) AS rn, COUNT(*) OVER() AS "TotalRecords"
            FROM Filtered
        )
        SELECT "CouponId","CouponCode","CouponName","Description","DiscountType","DiscountValue",
               "StartDate","EndDate","MaxUsageCount","UsageCount","MaxUsagePerUser","IsActive","CreatedDate","TotalRecords"
        FROM Sorted WHERE rn BETWEEN ($3+1) AND ($3+$4) ORDER BY rn',
        _sortcolumn, _sortdirection);
    RETURN QUERY EXECUTE _sql USING _searchterm, _searchvalue, _offset, _pagesize;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveCoupon
CREATE OR REPLACE FUNCTION "SP_SaveCoupon"(
    _couponid BIGINT DEFAULT 0, _couponcode VARCHAR(50) DEFAULT NULL, _couponname VARCHAR(100) DEFAULT NULL,
    _description VARCHAR(250) DEFAULT NULL, _discounttype VARCHAR(20) DEFAULT NULL,
    _discountvalue DECIMAL(10,2) DEFAULT NULL, _startdate TIMESTAMP DEFAULT NULL, _enddate TIMESTAMP DEFAULT NULL,
    _maxusagecount INT DEFAULT NULL, _maxusageperuser INT DEFAULT NULL,
    _appliesto VARCHAR(50) DEFAULT 'All', _appliestoid BIGINT DEFAULT NULL,
    _isactive BOOLEAN DEFAULT TRUE, _createdby BIGINT DEFAULT NULL
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    IF _couponid = 0 THEN
        INSERT INTO "Coupons"("CouponCode","CouponName","Description","DiscountType","DiscountValue",
            "StartDate","EndDate","MaxUsageCount","MaxUsagePerUser","IsActive","CreatedDate","CreatedBy")
        VALUES(_couponcode,_couponname,_description,_discounttype,_discountvalue,
            _startdate,_enddate,_maxusagecount,_maxusageperuser,TRUE,CURRENT_TIMESTAMP,_createdby)
        RETURNING "CouponId" INTO _newid;
        RETURN QUERY SELECT _newid;
    ELSE
        UPDATE "Coupons" SET "CouponCode"=_couponcode,"CouponName"=_couponname,"Description"=_description,
            "DiscountType"=_discounttype,"DiscountValue"=_discountvalue,"StartDate"=_startdate,"EndDate"=_enddate,
            "MaxUsageCount"=_maxusagecount,"MaxUsagePerUser"=_maxusageperuser,"IsActive"=_isactive
        WHERE "CouponId" = _couponid;
        RETURN QUERY SELECT 0::BIGINT;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCouponUsedDetails
CREATE OR REPLACE FUNCTION "SP_GetCouponUsedDetails"(_couponid BIGINT)
RETURNS TABLE(
    "CustomerId" BIGINT, "FirstName" TEXT, "LastName" TEXT, "Email" TEXT,
    "CouponCode" VARCHAR(50), "CouponName" VARCHAR(100), "Description" VARCHAR(250),
    "DiscountType" VARCHAR(20), "DiscountValue" DECIMAL(10,2), "StartDate" TIMESTAMP,
    "EndDate" TIMESTAMP, "MaxUsageCount" INT, "MaxUsagePerUser" INT,
    "UsageCount" INT, "LastUsedDate" TIMESTAMP, "CouponUsedTime" INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT CC."CustomerId", CS."FirstName", CS."LastName", CS."Email",
        C."CouponCode", C."CouponName", C."Description", C."DiscountType",
        C."DiscountValue", C."StartDate", C."EndDate", C."MaxUsageCount", C."MaxUsagePerUser",
        CC."UsageCount", CC."LastUsedDate", C."UsageCount" AS "CouponUsedTime"
    FROM "Coupons" C
    LEFT JOIN "CouponCustomers" CC ON C."CouponId" = CC."CouponId"
    LEFT JOIN "Customers" CS ON CS."CustomerId" = CC."CustomerId"
    WHERE C."CouponId" = _couponid;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- CUSTOMER FUNCTIONS
-- =====================================================

-- SP_GetCustomerByEmail
CREATE OR REPLACE FUNCTION "SP_GetCustomerByEmail"(_customeremail TEXT)
RETURNS SETOF "Customers" AS $$
BEGIN
    IF EXISTS(SELECT 1 FROM "Customers" WHERE LOWER("Email") = LOWER(_customeremail) AND "IsDeleted" = FALSE) THEN
        RETURN QUERY SELECT * FROM "Customers" WHERE LOWER("Email") = LOWER(_customeremail) AND "IsDeleted" = FALSE LIMIT 1;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCustomerById
CREATE OR REPLACE FUNCTION "SP_GetCustomerById"(_customerid BIGINT)
RETURNS SETOF "Customers" AS $$
BEGIN
    RETURN QUERY SELECT * FROM "Customers" WHERE "CustomerId" = _customerid AND "IsDeleted" != TRUE;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCustomerList
CREATE OR REPLACE FUNCTION "SP_GetCustomerList"(
    _pagenumber INT DEFAULT 1, _pagesize INT DEFAULT 10,
    _searchterm VARCHAR(100) DEFAULT '', _sortcolumn VARCHAR(50) DEFAULT 'FirstName',
    _sortdirection VARCHAR(4) DEFAULT 'ASC'
)
RETURNS TABLE(
    "CustomerId" BIGINT, "FirstName" TEXT, "LastName" TEXT, "Email" TEXT,
    "CountryCode" VARCHAR(3), "ContactNumber" VARCHAR(15),
    "IsWelcomeMailSent" BOOLEAN, "TotalRecords" BIGINT
) AS $$
DECLARE _offset INT; _searchvalue TEXT; _sql TEXT;
BEGIN
    IF _pagenumber < 1 THEN _pagenumber := 1; END IF;
    IF _pagesize < 1 THEN _pagesize := 10; END IF;
    IF _pagesize > 1000 THEN _pagesize := 1000; END IF;
    IF _sortcolumn NOT IN ('CustomerId','FirstName','LastName','Email','ContactNumber') THEN _sortcolumn := 'FirstName'; END IF;
    IF UPPER(_sortdirection) NOT IN ('ASC','DESC') THEN _sortdirection := 'ASC'; END IF;
    _offset := (_pagenumber - 1) * _pagesize;
    _searchvalue := '%' || TRIM(_searchterm) || '%';

    _sql := format(
        'WITH Filtered AS (
            SELECT "CustomerId","FirstName","LastName","Email","CountryCode","ContactNumber","IsWelcomeMailSent"
            FROM "Customers" WHERE "IsDeleted" = FALSE
              AND ($1 = '''' OR "FirstName" ILIKE $2 OR "ContactNumber" ILIKE $2 OR "LastName" ILIKE $2 OR "Email" ILIKE $2)
        ),
        Sorted AS (SELECT *, ROW_NUMBER() OVER (ORDER BY %I %s) AS rn, COUNT(*) OVER() AS "TotalRecords" FROM Filtered)
        SELECT "CustomerId","FirstName","LastName","Email","CountryCode","ContactNumber","IsWelcomeMailSent","TotalRecords"
        FROM Sorted WHERE rn BETWEEN ($3+1) AND ($3+$4) ORDER BY rn',
        _sortcolumn, _sortdirection);
    RETURN QUERY EXECUTE _sql USING _searchterm, _searchvalue, _offset, _pagesize;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveCustomer
CREATE OR REPLACE FUNCTION "SP_SaveCustomer"(
    _customerid BIGINT, _firstname TEXT, _lastname TEXT, _email VARCHAR(250),
    _countrycode VARCHAR(10), _contactnumber VARCHAR(15), _createdby BIGINT,
    _passwordhash TEXT, _passwordsalt TEXT
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    IF EXISTS (SELECT 1 FROM "Customers" WHERE "Email" = _email AND (_customerid = 0 OR "CustomerId" <> _customerid) AND "IsDeleted" = FALSE) THEN
        RETURN QUERY SELECT -1::BIGINT; RETURN;
    END IF;
    IF _customerid = 0 THEN
        INSERT INTO "Customers"("FirstName","LastName","Email","CountryCode","ContactNumber","PasswordHash","PasswordSalt","CreatedBy","CreatedAt")
        VALUES(_firstname,_lastname,_email,_countrycode,_contactnumber,_passwordhash,_passwordsalt,_createdby,NOW())
        RETURNING "CustomerId" INTO _newid;
        RETURN QUERY SELECT _newid;
    ELSE
        UPDATE "Customers" SET "FirstName"=_firstname,"LastName"=_lastname,"Email"=_email,
            "CountryCode"=_countrycode,"ContactNumber"=_contactnumber,"UpdatedBy"=_createdby,"UpdatedAt"=NOW()
        WHERE "CustomerId" = _customerid;
        RETURN QUERY SELECT _customerid;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_UpdateCustomerWelcomeStatus
CREATE OR REPLACE FUNCTION "SP_UpdateCustomerWelcomeStatus"(_customerid BIGINT, _emailtemplate TEXT)
RETURNS TABLE(result INT) AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM "Customers" WHERE "CustomerId" = _customerid AND "IsWelcomeMailSent" = FALSE) THEN
        UPDATE "Customers" SET "IsWelcomeMailSent" = TRUE WHERE "CustomerId" = _customerid;
        INSERT INTO "CustomerWelcomeEmails"("CustomerId","EmailTemplate") VALUES(_customerid, _emailtemplate);
    END IF;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_DeleteCustomer
CREATE OR REPLACE FUNCTION "SP_DeleteCustomer"(_customerid BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "Customers" SET "IsDeleted" = TRUE WHERE "CustomerId" = _customerid;
    RETURN QUERY SELECT 0;
END;
$$ LANGUAGE plpgsql;

-- SP_UpdateFCMToken
CREATE OR REPLACE FUNCTION "SP_UpdateFCMToken"(_customerid BIGINT, _fcmtoken TEXT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "Customers" SET "FCMToken" = _fcmtoken WHERE "CustomerId" = _customerid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;


-- =====================================================
-- CUSTOMER TOKEN FUNCTIONS
-- =====================================================

-- SP_InsertCustomerToken
CREATE OR REPLACE FUNCTION "SP_InsertCustomerToken"(
    _customerid BIGINT, _jwttoken TEXT, _jwtcreateddate TIMESTAMP, _jwtexpirydate TIMESTAMP
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    INSERT INTO "CustomerTokens"("CustomerId","JwtToken","JwtCreatedDate","JwtExpiryDate")
    VALUES(_customerid, _jwttoken, _jwtcreateddate, _jwtexpirydate);
    _newid := currval(pg_get_serial_sequence('"CustomerTokens"', 'CustomerTokenId'));
    RETURN QUERY SELECT _newid;
END;
$$ LANGUAGE plpgsql;

-- SP_ValidateCustomerToken
CREATE OR REPLACE FUNCTION "SP_ValidateCustomerToken"(_jwttoken TEXT)
RETURNS TABLE("ValidationResult" INT) AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM "CustomerTokens" WHERE "JwtToken" = _jwttoken AND "JwtExpiryDate" > CURRENT_TIMESTAMP) THEN
        RETURN QUERY SELECT 1;
    ELSE
        RETURN QUERY SELECT -1;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_GetCustomerDataByJWT
CREATE OR REPLACE FUNCTION "SP_GetCustomerDataByJWT"(_jwttoken TEXT)
RETURNS SETOF "CustomerTokens" AS $$
BEGIN
    IF EXISTS(SELECT 1 FROM "CustomerTokens" WHERE "JwtToken" LIKE '%' || _jwttoken || '%') THEN
        RETURN QUERY SELECT * FROM "CustomerTokens" WHERE "JwtToken" LIKE '%' || _jwttoken || '%';
    END IF;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- ORDER FUNCTIONS
-- =====================================================

-- SP_CheckoutCart
CREATE OR REPLACE FUNCTION "SP_CheckoutCart"(
    _cartsessionid VARCHAR(255), _customerid INT DEFAULT NULL,
    _billingaddress TEXT DEFAULT NULL, _shippingaddress TEXT DEFAULT NULL,
    _shippingsameasbilling BOOLEAN DEFAULT TRUE, _paymentmethod VARCHAR(100) DEFAULT NULL,
    _ordernotes TEXT DEFAULT NULL, _paymentintentid TEXT DEFAULT NULL,
    _stripepaymentstatus TEXT DEFAULT NULL
)
RETURNS TABLE("OrderId" BIGINT, "OrderNumber" VARCHAR(50), "Total" DECIMAL(18,2), "Result" INT, "Message" TEXT) AS $$
DECLARE
    _subtotal DECIMAL(18,2); _discountamount DECIMAL(18,2); _couponcode VARCHAR(100);
    _taxrate DECIMAL(5,4); _taxamount DECIMAL(18,2); _shippingamount DECIMAL(18,2) := 0.00;
    _total DECIMAL(18,2); _orderid BIGINT; _ordernumber VARCHAR(50);
    _stockvalidation INT := 0; _subscriptiondiscountamount DECIMAL(18,4); _currentplanid BIGINT;
    _discountpercentage DECIMAL(18,4); _ordercounter INT; _couponid BIGINT;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "ProductCart" WHERE "SessionId" = _cartsessionid AND "Status" = 0) THEN
        RETURN QUERY SELECT NULL::BIGINT, NULL::VARCHAR(50), NULL::DECIMAL(18,2), -1, 'Cart not found or already processed'::TEXT;
        RETURN;
    END IF;

    SELECT COUNT(*) INTO _stockvalidation
    FROM "ProductCartItem" ci
    INNER JOIN "ProductCart" pc ON ci."CartSessionId" = pc."Id"
    INNER JOIN "Stock" s ON s."ProductID" = ci."ProductId"
    WHERE pc."SessionId" = _cartsessionid AND ci."Active" = TRUE AND s."AvailableQuantity" < ci."Quantity";

    IF _stockvalidation > 0 THEN
        RETURN QUERY SELECT NULL::BIGINT, NULL::VARCHAR(50), NULL::DECIMAL(18,2), -2, 'Insufficient stock'::TEXT;
        RETURN;
    END IF;

    SELECT pc."SubTotal", pc."DiscountAmount", pc."CouponCode"
    INTO _subtotal, _discountamount, _couponcode
    FROM "ProductCart" pc WHERE pc."SessionId" = _cartsessionid AND pc."Status" = 0;

    SELECT COALESCE(sp."PolicyDescription"::DECIMAL, 0) INTO _taxrate
    FROM "SitePolicies" sp WHERE sp."PolicyName" LIKE '%TaxPercentage%';

    _taxamount := (_subtotal - COALESCE(_discountamount,0)) * COALESCE(_taxrate, 0);

    SELECT P."PlanId" INTO _currentplanid FROM "CustomerSubscriptions" CS
    INNER JOIN "Plans" P ON CS."PlanId" = P."PlanId"
    WHERE CS."CustomerId" = _customerid AND CS."SubscriptionStatus" = 'active';

    SELECT PB."BenefitValue"::DECIMAL INTO _discountpercentage FROM "CustomerSubscriptions" CS
    INNER JOIN "Plans" P ON CS."PlanId" = P."PlanId"
    INNER JOIN "PlanBenefits" PB ON P."PlanId" = PB."PlanId"
    WHERE PB."BenefitKey" = 'discount_percent' AND CS."CustomerId" = _customerid AND CS."SubscriptionStatus" = 'active';

    _subscriptiondiscountamount := COALESCE(_subtotal * _discountpercentage / 100, 0);
    _total := (_subtotal - (COALESCE(_discountamount,0) + _subscriptiondiscountamount)) + _taxamount + _shippingamount;

    SELECT COALESCE(MAX(CAST(SUBSTRING("OrderNumber" FROM 4) AS INT)), 0) + 1
    INTO _ordercounter FROM "Orders" WHERE "OrderNumber" LIKE 'ORD%';

    _ordernumber := 'ORD' || LPAD(_ordercounter::TEXT, 6, '0');

    INSERT INTO "Orders"("CartSessionId","CustomerId","OrderNumber","OrderStatus","BillingAddress","ShippingAddress",
        "ShippingSameAsBilling","SubTotal","DiscountAmount","CouponCode","TaxAmount","ShippingAmount","Total",
        "PaymentMethod","OrderNotes","PaymentIntentId","StripePaymentStatus","SubscriptionDiscount","CurrentPlanId")
    VALUES(_cartsessionid,_customerid,_ordernumber,1,_billingaddress,_shippingaddress,
        _shippingsameasbilling,_subtotal,COALESCE(_discountamount,0),_couponcode,_taxamount,_shippingamount,_total,
        _paymentmethod,_ordernotes,_paymentintentid,_stripepaymentstatus,_subscriptiondiscountamount,_currentplanid)
    RETURNING "Id" INTO _orderid;

    INSERT INTO "OrderDetail"("OrderId","CartItemProductId","ProductId","ProductName","ProductSKU","ProductDescription","UnitPrice","Quantity","Discount")
    SELECT _orderid, ci."Id", ci."ProductId", ps."ProductName", ps."SKU", ps."ProductDescription", ci."Price", ci."Quantity", 0.00
    FROM "ProductCartItem" ci
    INNER JOIN "ProductCart" pc ON ci."CartSessionId" = pc."Id"
    INNER JOIN "Products" ps ON ps."ProductID" = ci."ProductId"
    WHERE pc."SessionId" = _cartsessionid AND ci."Active" = TRUE;

    UPDATE "Stock" s SET "ReservedQuantity" = s."ReservedQuantity" + ci."Quantity", "UpdatedAt" = CURRENT_TIMESTAMP, "UpdatedBy" = _customerid
    FROM "ProductCartItem" ci INNER JOIN "ProductCart" pc ON ci."CartSessionId" = pc."Id"
    WHERE s."ProductID" = ci."ProductId" AND pc."SessionId" = _cartsessionid AND ci."Active" = TRUE;

    UPDATE "Coupons" c SET "UsageCount" = c."UsageCount" + 1
    FROM "ProductCart" pc WHERE pc."CouponId" = c."CouponId" AND pc."SessionId" = _cartsessionid AND pc."CouponId" IS NOT NULL;

    UPDATE "ProductCart" SET "Status" = 1, "UpdatedAt" = CURRENT_TIMESTAMP WHERE "SessionId" = _cartsessionid;
    UPDATE "ProductCartItem" pci SET "Active" = FALSE, "UpdatedAt" = CURRENT_TIMESTAMP
    FROM "ProductCart" pc WHERE pc."Id" = pci."CartSessionId" AND pc."SessionId" = _cartsessionid;

    INSERT INTO "OrderStatusHistory"("OrderId","NewStatus","StatusComment") VALUES(_orderid, 1, 'Order created - Stock reserved');

    IF _couponcode IS NOT NULL AND _customerid IS NOT NULL THEN
        SELECT c."CouponId" INTO _couponid FROM "Coupons" c WHERE c."CouponCode" = _couponcode;
        IF EXISTS (SELECT 1 FROM "CouponCustomers" WHERE "CouponId" = _couponid AND "CustomerId" = _customerid) THEN
            UPDATE "CouponCustomers" SET "UsageCount" = "UsageCount" + 1, "LastUsedDate" = CURRENT_TIMESTAMP, "CartSessionId" = _cartsessionid
            WHERE "CouponId" = _couponid AND "CustomerId" = _customerid;
        ELSE
            INSERT INTO "CouponCustomers"("CouponId","CustomerId","UsageCount","LastUsedDate","CartSessionId")
            VALUES(_couponid, _customerid, 1, CURRENT_TIMESTAMP, _cartsessionid);
        END IF;
    END IF;

    RETURN QUERY SELECT _orderid, _ordernumber, _total, 1, 'Order created successfully'::TEXT;
END;
$$ LANGUAGE plpgsql;

-- SP_GetOrderList
CREATE OR REPLACE FUNCTION "SP_GetOrderList"(_customerid BIGINT)
RETURNS TABLE(
    "Id" BIGINT, "OrderNumber" VARCHAR(50), "OrderStatus" SMALLINT, "Total" DECIMAL(18,2),
    "CreatedAt" TIMESTAMP, "ProductId" INT, "ProductName" VARCHAR(255),
    "PaymentStatus" SMALLINT, "StripePaymentStatus" TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT O."Id", O."OrderNumber", O."OrderStatus", O."Total", O."CreatedAt",
        OD."ProductId", OD."ProductName", O."PaymentStatus", O."StripePaymentStatus"
    FROM "Orders" O INNER JOIN "OrderDetail" OD ON O."Id" = OD."OrderId"
    WHERE O."CustomerId" = _customerid ORDER BY O."CreatedAt" DESC;
END;
$$ LANGUAGE plpgsql;

-- SP_GetOrderDetail
CREATE OR REPLACE FUNCTION "SP_GetOrderDetail"(_orderid BIGINT)
RETURNS TABLE(
    "Id" BIGINT, "ProductId" INT, "ProductName" VARCHAR(255), "ProductDescription" TEXT,
    "UnitPrice" DECIMAL(18,2), "Quantity" INT, "Discount" DECIMAL(18,2), "LineTotal" DECIMAL(18,2),
    "Total" DECIMAL(18,2), "TaxAmount" DECIMAL(18,2), "ShippingAmount" DECIMAL(18,2),
    "DiscountAmount" DECIMAL(18,2), "PaymentMethod" VARCHAR(100), "PaymentStatus" SMALLINT,
    "CreatedAt" TIMESTAMP, "ImageUrl" VARCHAR(500), "CouponCode" VARCHAR(100),
    "StripePaymentStatus" TEXT, "OrderStatus" SMALLINT, "OrderCreatedAt" TIMESTAMP,
    "SubscriptionDiscount" DECIMAL(18,4), "PlanName" VARCHAR(100), "SubscriptionBenefitValue" VARCHAR(100)
) AS $$
BEGIN
    RETURN QUERY
    SELECT O."Id", OD."ProductId", OD."ProductName", OD."ProductDescription",
        OD."UnitPrice", OD."Quantity", O."DiscountAmount", OD."LineTotal", O."SubTotal",
        O."TaxAmount", O."ShippingAmount", O."DiscountAmount", O."PaymentMethod",
        O."PaymentStatus", O."CreatedAt", PIM."ImageUrl", O."CouponCode",
        O."StripePaymentStatus", O."OrderStatus", O."CreatedAt",
        O."SubscriptionDiscount", PP."PlanName", PB."BenefitValue"
    FROM "Orders" O
    INNER JOIN "OrderDetail" OD ON O."Id" = OD."OrderId"
    INNER JOIN "Products" P ON OD."ProductId" = P."ProductID"
    LEFT JOIN "ProductImages" PIM ON PIM."ProductID" = P."ProductID" AND PIM."IsPrimary" = TRUE
    LEFT JOIN "Plans" PP ON O."CurrentPlanId" = PP."PlanId"
    LEFT JOIN "PlanBenefits" PB ON PP."PlanId" = PB."PlanId" AND PB."BenefitKey" = 'discount_percent'
    WHERE O."Id" = _orderid;
END;
$$ LANGUAGE plpgsql;

-- SP_GetAdminOrderList
CREATE OR REPLACE FUNCTION "SP_GetAdminOrderList"(
    _pagenumber INT DEFAULT 1, _pagesize INT DEFAULT 10,
    _searchterm VARCHAR(100) DEFAULT '', _sortcolumn VARCHAR(50) DEFAULT 'CreatedAt',
    _sortdirection VARCHAR(4) DEFAULT 'DESC'
)
RETURNS TABLE(
    "Id" BIGINT, "OrderNumber" VARCHAR(50), "OrderStatus" SMALLINT, "PaymentStatus" SMALLINT,
    "BillingAddress" TEXT, "SubTotal" DECIMAL(18,2), "TaxAmount" DECIMAL(18,2),
    "DiscountAmount" DECIMAL(18,2), "Total" DECIMAL(18,2), "CustomerId" BIGINT,
    "FullName" TEXT, "TotalRecords" BIGINT
) AS $$
DECLARE _offset INT; _searchvalue TEXT; _sql TEXT;
BEGIN
    IF _pagenumber < 1 THEN _pagenumber := 1; END IF;
    IF _pagesize < 1 THEN _pagesize := 10; END IF;
    IF _pagesize > 1000 THEN _pagesize := 1000; END IF;
    IF _sortcolumn NOT IN ('OrderNumber','OrderStatus','BillingAddress','SubTotal','TaxAmount','DiscountAmount','Total','CustomerId','CreatedAt') THEN _sortcolumn := 'CreatedAt'; END IF;
    IF UPPER(_sortdirection) NOT IN ('ASC','DESC') THEN _sortdirection := 'DESC'; END IF;
    _offset := (_pagenumber - 1) * _pagesize;
    _searchvalue := '%' || TRIM(_searchterm) || '%';

    _sql := format(
        'WITH Filtered AS (
            SELECT O."Id", O."OrderNumber", O."OrderStatus", O."PaymentStatus", O."BillingAddress",
                O."SubTotal", O."TaxAmount", O."DiscountAmount", O."Total", O."CustomerId",
                C."FirstName" || '' '' || C."LastName" AS "FullName", O."CreatedAt"
            FROM "Orders" O INNER JOIN "Customers" C ON O."CustomerId" = C."CustomerId"
            WHERE ($1 = '''' OR O."OrderNumber" ILIKE $2 OR O."SubTotal"::TEXT ILIKE $2 OR
                O."TaxAmount"::TEXT ILIKE $2 OR O."DiscountAmount"::TEXT ILIKE $2 OR
                O."Total"::TEXT ILIKE $2 OR C."FirstName" ILIKE $2)
        ),
        Sorted AS (SELECT *, ROW_NUMBER() OVER (ORDER BY %I %s) AS rn, COUNT(*) OVER() AS "TotalRecords" FROM Filtered)
        SELECT "Id","OrderNumber","OrderStatus","PaymentStatus","BillingAddress","SubTotal",
            "TaxAmount","DiscountAmount","Total","CustomerId","FullName","TotalRecords"
        FROM Sorted WHERE rn BETWEEN ($3+1) AND ($3+$4) ORDER BY rn',
        _sortcolumn, _sortdirection);
    RETURN QUERY EXECUTE _sql USING _searchterm, _searchvalue, _offset, _pagesize;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveStripeWebHookEvents
CREATE OR REPLACE FUNCTION "SP_SaveStripeWebHookEvents"(
    _stripeeventid VARCHAR(255), _eventtype VARCHAR(100), _apiversion VARCHAR(50),
    _paymentintentid VARCHAR(255) DEFAULT NULL, _checkoutsessionid VARCHAR(255) DEFAULT NULL,
    _chargeid VARCHAR(255) DEFAULT NULL, _payloadjson TEXT DEFAULT NULL
)
RETURNS TABLE(result INT) AS $$
DECLARE _ordernumber VARCHAR(100); _paymentstatus SMALLINT := 0; _stripepaymentstatus VARCHAR(50);
BEGIN
    IF COALESCE(_paymentintentid, '') <> '' THEN
        SELECT o."OrderNumber" INTO _ordernumber FROM "Orders" o WHERE o."PaymentIntentId" = _paymentintentid;
    END IF;

    INSERT INTO "StripeWebhookEvents"("StripeEventId","EventType","ApiVersion","PaymentIntentId",
        "CheckoutSessionId","ChargeId","PayloadJson","Processed","OrderNumber")
    VALUES(_stripeeventid,_eventtype,_apiversion,_paymentintentid,
        _checkoutsessionid,_chargeid,_payloadjson,FALSE,_ordernumber);

    IF _eventtype = 'payment_intent.succeeded' THEN _paymentstatus := 1; _stripepaymentstatus := 'succeeded';
    ELSIF _eventtype = 'payment_intent.payment_failed' THEN _paymentstatus := 2; _stripepaymentstatus := 'failed';
    ELSIF _eventtype = 'payment_intent.canceled' THEN _paymentstatus := 3; _stripepaymentstatus := 'canceled';
    END IF;

    IF _paymentstatus IS NOT NULL AND COALESCE(_paymentintentid,'') <> '' THEN
        UPDATE "Orders" SET "PaymentStatus" = _paymentstatus, "StripePaymentStatus" = _stripepaymentstatus,
            "PaymentDate" = CASE WHEN _paymentstatus = 1 THEN CURRENT_TIMESTAMP ELSE "PaymentDate" END,
            "UpdatedAt" = CURRENT_TIMESTAMP
        WHERE "PaymentIntentId" = _paymentintentid;
    END IF;

    UPDATE "StripeWebhookEvents" SET "Processed" = TRUE, "ProcessedAt" = CURRENT_TIMESTAMP
    WHERE "StripeEventId" = _stripeeventid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;


-- =====================================================
-- DASHBOARD FUNCTIONS
-- =====================================================

-- SP_GetDashboardCounts
CREATE OR REPLACE FUNCTION "SP_GetDashboardCounts"()
RETURNS TABLE("Users" BIGINT, "Orders" BIGINT, "Customers" BIGINT, "Products" BIGINT, "Categories" BIGINT) AS $$
BEGIN
    RETURN QUERY SELECT
        (SELECT COUNT("UserId") FROM "UserMaster" WHERE "IsDeleted" = FALSE),
        (SELECT COUNT("Id") FROM "Orders"),
        (SELECT COUNT("CustomerId") FROM "Customers" WHERE "IsDeleted" = FALSE),
        (SELECT COUNT("ProductID") FROM "Products"),
        (SELECT COUNT("CategoryID") FROM "Categories");
END;
$$ LANGUAGE plpgsql;

-- SP_MonhtlySalesAmount (keeping original typo in name)
CREATE OR REPLACE FUNCTION "SP_MonhtlySalesAmount"(_year INT DEFAULT NULL)
RETURNS TABLE("SalesYear" INT, "SalesMonth" INT, "MonthName" TEXT, "MonthlyTotal" DECIMAL) AS $$
DECLARE _startdate DATE; _enddate DATE;
BEGIN
    IF _year IS NULL THEN _year := EXTRACT(YEAR FROM CURRENT_DATE)::INT; END IF;
    _startdate := make_date(_year, 1, 1);
    _enddate := _startdate + INTERVAL '1 year';

    RETURN QUERY
    SELECT _year, EXTRACT(MONTH FROM "CreatedAt")::INT,
        TO_CHAR("CreatedAt", 'Month')::TEXT, SUM("Total")
    FROM "Orders"
    WHERE "CreatedAt" >= _startdate AND "CreatedAt" < _enddate
    GROUP BY EXTRACT(MONTH FROM "CreatedAt"), TO_CHAR("CreatedAt", 'Month')
    ORDER BY EXTRACT(MONTH FROM "CreatedAt");
END;
$$ LANGUAGE plpgsql;

-- SP_GetMostOrderedProducts
CREATE OR REPLACE FUNCTION "SP_GetMostOrderedProducts"(_filter BIGINT DEFAULT NULL)
RETURNS TABLE(
    "ProductID" INT, "ProductName" VARCHAR(200), "ProductDescription" TEXT,
    "OrderedCount" BIGINT, "ProductImage" VARCHAR(500)
) AS $$
BEGIN
    IF _filter IS NULL THEN _filter := 0; END IF;
    RETURN QUERY
    SELECT PS."ProductID", PS."ProductName", PS."ProductDescription",
        SUM(OD."Quantity")::BIGINT, PIM."ImageUrl"
    FROM "OrderDetail" OD
    INNER JOIN "Products" PS ON OD."ProductId" = PS."ProductID"
    LEFT JOIN "ProductImages" PIM ON PS."ProductID" = PIM."ProductID" AND PIM."IsDeleted" = FALSE AND PIM."IsPrimary" = TRUE
    WHERE (
        (_filter = 0 AND OD."CreatedAt"::DATE = CURRENT_DATE)
        OR (_filter = 1 AND EXTRACT(YEAR FROM OD."CreatedAt") = EXTRACT(YEAR FROM CURRENT_DATE)
            AND EXTRACT(MONTH FROM OD."CreatedAt") = EXTRACT(MONTH FROM CURRENT_DATE))
        OR (_filter = 2 AND EXTRACT(YEAR FROM OD."CreatedAt") = EXTRACT(YEAR FROM CURRENT_DATE))
    )
    GROUP BY PS."ProductID", PS."ProductName", PS."ProductDescription", PIM."ImageUrl"
    ORDER BY SUM(OD."Quantity") DESC;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- CONTACT US FUNCTIONS
-- =====================================================

-- SP_SaveContactRequest
CREATE OR REPLACE FUNCTION "SP_SaveContactRequest"(
    _contactusid BIGINT, _customername TEXT, _customeremail TEXT,
    _contactsubject TEXT, _customermessage TEXT, _adminmessage TEXT
)
RETURNS TABLE(result INT) AS $$
BEGIN
    IF COALESCE(_contactusid, 0) > 0 THEN
        UPDATE "ContactUsRequests" SET "IsReplied" = TRUE, "IsRepliedAt" = CURRENT_TIMESTAMP, "AdminMessage" = _adminmessage
        WHERE "ContactUsId" = _contactusid;
        RETURN QUERY SELECT 1;
    ELSE
        INSERT INTO "ContactUsRequests"("CustomerName","CustomerEmail","ContactSubject","CustomerMessage")
        VALUES(_customername, _customeremail, _contactsubject, _customermessage);
        RETURN QUERY SELECT 0;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_GetContactUsRequestList
CREATE OR REPLACE FUNCTION "SP_GetContactUsRequestList"(
    _pagenumber INT DEFAULT 1, _pagesize INT DEFAULT 10,
    _searchterm VARCHAR(100) DEFAULT '', _sortcolumn VARCHAR(50) DEFAULT 'CustomerName',
    _sortdirection VARCHAR(4) DEFAULT 'ASC'
)
RETURNS TABLE(
    "ContactUsId" BIGINT, "CustomerName" TEXT, "CustomerEmail" TEXT,
    "ContactSubject" TEXT, "CustomerMessage" TEXT, "IsReplied" BOOLEAN,
    "AdminMessage" TEXT, "TotalRecords" BIGINT
) AS $$
DECLARE _offset INT; _searchvalue TEXT; _sql TEXT;
BEGIN
    IF _pagenumber < 1 THEN _pagenumber := 1; END IF;
    IF _pagesize < 1 THEN _pagesize := 10; END IF;
    IF _pagesize > 1000 THEN _pagesize := 1000; END IF;
    IF _sortcolumn NOT IN ('ContactUsId','CustomerName','CustomerEmail','ContactSubject','CustomerMessage','AdminMessage') THEN _sortcolumn := 'CustomerName'; END IF;
    IF UPPER(_sortdirection) NOT IN ('ASC','DESC') THEN _sortdirection := 'ASC'; END IF;
    _offset := (_pagenumber - 1) * _pagesize;
    _searchvalue := '%' || TRIM(_searchterm) || '%';

    _sql := format(
        'WITH Filtered AS (
            SELECT "ContactUsId","CustomerName","CustomerEmail","ContactSubject","CustomerMessage","IsReplied","AdminMessage"
            FROM "ContactUsRequests" WHERE "IsDeleted" = 0
              AND ($1 = '''' OR "CustomerName" ILIKE $2 OR "CustomerEmail" ILIKE $2 OR "ContactSubject" ILIKE $2 OR "CustomerMessage" ILIKE $2)
        ),
        Sorted AS (SELECT *, ROW_NUMBER() OVER (ORDER BY %I %s) AS rn, COUNT(*) OVER() AS "TotalRecords" FROM Filtered)
        SELECT "ContactUsId","CustomerName","CustomerEmail","ContactSubject","CustomerMessage","IsReplied","AdminMessage","TotalRecords"
        FROM Sorted WHERE rn BETWEEN ($3+1) AND ($3+$4) ORDER BY rn',
        _sortcolumn, _sortdirection);
    RETURN QUERY EXECUTE _sql USING _searchterm, _searchvalue, _offset, _pagesize;
END;
$$ LANGUAGE plpgsql;

-- SP_DeleteContactRequest
CREATE OR REPLACE FUNCTION "SP_DeleteContactRequest"(_contactusid BIGINT)
RETURNS TABLE(result INT) AS $$
DECLARE _isreplied BOOLEAN;
BEGIN
    SELECT "IsReplied" INTO _isreplied FROM "ContactUsRequests" WHERE "ContactUsId" = _contactusid;
    IF _isreplied = FALSE THEN
        RETURN QUERY SELECT -1;
    ELSE
        UPDATE "ContactUsRequests" SET "IsDeleted" = 1 WHERE "ContactUsId" = _contactusid;
        RETURN QUERY SELECT 1;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- WISHLIST FUNCTIONS
-- =====================================================

-- SP_SaveWishlist
CREATE OR REPLACE FUNCTION "SP_SaveWishlist"(_productid BIGINT, _customerid BIGINT)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _newid BIGINT;
BEGIN
    IF EXISTS(SELECT 1 FROM "CustomerWishlist" WHERE "ProductId" = _productid AND "CustomerId" = _customerid AND "IsRemoved" = FALSE) THEN
        RETURN QUERY SELECT -1::BIGINT;
    ELSE
        INSERT INTO "CustomerWishlist"("ProductId","CustomerId") VALUES(_productid, _customerid)
        RETURNING "WishlistId" INTO _newid;
        RETURN QUERY SELECT _newid;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_RemoveFromWhislist (keeping original typo in name)
CREATE OR REPLACE FUNCTION "SP_RemoveFromWhislist"(_productid BIGINT, _customerid BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "CustomerWishlist" SET "IsRemoved" = TRUE WHERE "CustomerId" = _customerid AND "ProductId" = _productid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_GetWishlistProductList
CREATE OR REPLACE FUNCTION "SP_GetWishlistProductList"(_customerid BIGINT DEFAULT 0)
RETURNS TABLE(
    "ProductID" INT, "ProductName" VARCHAR(200), "Price" DECIMAL(10,2),
    "ProductDescription" TEXT, "ProductImage" VARCHAR(500), "CategoryID" INT,
    "CategoryName" VARCHAR(100), "SKU" VARCHAR(50), "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP, "IsWishlistItem" BOOLEAN
) AS $$
BEGIN
    RETURN QUERY
    SELECT P."ProductID", P."ProductName", P."Price", P."ProductDescription",
        PIM."ImageUrl", P."CategoryID", C."CategoryName", P."SKU", P."CreatedAt", P."UpdatedAt",
        CASE WHEN CW."ProductId" IS NOT NULL THEN TRUE ELSE FALSE END
    FROM "Products" P
    INNER JOIN "CustomerWishlist" CW ON P."ProductID" = CW."ProductId" AND CW."CustomerId" = _customerid AND CW."IsRemoved" = FALSE
    LEFT JOIN "ProductImages" PIM ON P."ProductID" = PIM."ProductID" AND PIM."IsPrimary" = TRUE
    LEFT JOIN "Categories" C ON C."CategoryID" = P."CategoryID";
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- ADDRESS FUNCTIONS
-- =====================================================

-- SP_SaveAddress
CREATE OR REPLACE FUNCTION "SP_SaveAddress"(
    _addressid BIGINT, _customerid BIGINT, _addressname TEXT, _fulladdress TEXT,
    _countryid BIGINT, _stateid BIGINT, _cityid BIGINT, _updatedby BIGINT
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _totalvalid BIGINT; _newid BIGINT;
BEGIN
    SELECT COUNT(*) INTO _totalvalid FROM "CustomerAddress" WHERE "IsDeleted" = FALSE AND "CustomerId" = _customerid;

    IF _addressid > 0 THEN
        UPDATE "CustomerAddress" SET "AddressName"=_addressname,"FullAddress"=_fulladdress,
            "CountryId"=_countryid,"StateId"=_stateid,"CityId"=_cityid,
            "UpdatedBy"=_updatedby,"UpdatedAt"=CURRENT_TIMESTAMP,"CustomerId"=_customerid
        WHERE "AddressId" = _addressid;
        _newid := 0;
    ELSE
        INSERT INTO "CustomerAddress"("AddressName","FullAddress","CountryId","StateId","CityId","CustomerId","IsDefault")
        VALUES(_addressname,_fulladdress,_countryid,_stateid,_cityid,_customerid,FALSE)
        RETURNING "AddressId" INTO _newid;
    END IF;

    IF _totalvalid = 0 THEN
        UPDATE "CustomerAddress" SET "IsDefault" = TRUE WHERE "AddressId" = COALESCE(_newid, _addressid);
    END IF;

    RETURN QUERY SELECT COALESCE(_newid, 0::BIGINT);
END;
$$ LANGUAGE plpgsql;

-- SP_GetAddressList
CREATE OR REPLACE FUNCTION "SP_GetAddressList"(_customerid BIGINT)
RETURNS TABLE(
    "AddressId" BIGINT, "AddressName" TEXT, "FullAddress" TEXT,
    "CountryName" TEXT, "StateName" TEXT, "CityName" TEXT,
    "IsDefault" BOOLEAN, "CountryId" BIGINT, "StateId" BIGINT, "CityId" BIGINT
) AS $$
BEGIN
    RETURN QUERY
    SELECT CA."AddressId", CA."AddressName", CA."FullAddress",
        CM."CountryName", SM."StateName", CMM."CityName",
        CA."IsDefault", CA."CountryId", CA."StateId", CA."CityId"
    FROM "CustomerAddress" CA
    INNER JOIN "CountryMaster" CM ON CA."CountryId" = CM."CountryId"
    INNER JOIN "StateMaster" SM ON CA."StateId" = SM."StateId"
    INNER JOIN "CityMaster" CMM ON CA."CityId" = CMM."CityId"
    WHERE CA."CustomerId" = _customerid AND CA."IsDeleted" = FALSE;
END;
$$ LANGUAGE plpgsql;

-- SP_DeleteAddress
CREATE OR REPLACE FUNCTION "SP_DeleteAddress"(_addressid BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    IF EXISTS(SELECT 1 FROM "CustomerAddress" WHERE "AddressId" = _addressid AND "IsDefault" = TRUE) THEN
        RETURN QUERY SELECT -1;
    ELSE
        UPDATE "CustomerAddress" SET "IsDeleted" = TRUE WHERE "AddressId" = _addressid;
        RETURN QUERY SELECT 1;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_MakeCustomerAddressDefault
CREATE OR REPLACE FUNCTION "SP_MakeCustomerAddressDefault"(_addressid BIGINT)
RETURNS TABLE(result INT) AS $$
DECLARE _customerid BIGINT;
BEGIN
    SELECT "CustomerId" INTO _customerid FROM "CustomerAddress" WHERE "AddressId" = _addressid;
    UPDATE "CustomerAddress" SET "IsDefault" = FALSE WHERE "CustomerId" = _customerid;
    UPDATE "CustomerAddress" SET "IsDefault" = TRUE WHERE "AddressId" = _addressid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- GENERAL / LOCATION FUNCTIONS
-- =====================================================

-- SP_GetCountryList
CREATE OR REPLACE FUNCTION "SP_GetCountryList"()
RETURNS SETOF "CountryMaster" AS $$
BEGIN RETURN QUERY SELECT * FROM "CountryMaster"; END;
$$ LANGUAGE plpgsql;

-- SP_GetStateList
CREATE OR REPLACE FUNCTION "SP_GetStateList"(_countryid BIGINT)
RETURNS SETOF "StateMaster" AS $$
BEGIN RETURN QUERY SELECT * FROM "StateMaster" WHERE "CountryId" = _countryid; END;
$$ LANGUAGE plpgsql;

-- SP_GetCityList
CREATE OR REPLACE FUNCTION "SP_GetCityList"(_stateid BIGINT)
RETURNS SETOF "CityMaster" AS $$
BEGIN RETURN QUERY SELECT * FROM "CityMaster" WHERE "StateId" = _stateid; END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- SITE POLICIES FUNCTIONS
-- =====================================================

-- SP_UpdatePolicy
CREATE OR REPLACE FUNCTION "SP_UpdatePolicy"(_sitepolicyid BIGINT, _policydescription TEXT, _updatedby BIGINT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "SitePolicies" SET "PolicyDescription" = _policydescription, "UpdatedBy" = _updatedby WHERE "SitePolicyId" = _sitepolicyid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_GetPolicies
CREATE OR REPLACE FUNCTION "SP_GetPolicies"()
RETURNS SETOF "SitePolicies" AS $$
BEGIN RETURN QUERY SELECT * FROM "SitePolicies"; END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- PLAN / SUBSCRIPTION FUNCTIONS
-- =====================================================

-- SP_GetPlans
CREATE OR REPLACE FUNCTION "SP_GetPlans"(_customerid BIGINT)
RETURNS TABLE(
    "PlanId" INT, "PlanName" VARCHAR(100), "StripeProductId" VARCHAR(100),
    "PlanDescription" VARCHAR(500), "StripePriceId" VARCHAR(100),
    "Amount" DECIMAL(10,2), "Currency" VARCHAR(10), "BillingInterval" VARCHAR(20),
    "Benefits" JSON, "ActivePlanId" INT, "CurrentStripeSubscriptionId" VARCHAR(100),
    "CurrentPeriodStart" TIMESTAMP, "CurrentPeriodEnd" TIMESTAMP,
    "CancelAtPeriodEnd" BOOLEAN, "IsCurrentPlan" INT
) AS $$
DECLARE
    _activeplanid INT; _currentsubid VARCHAR(100);
    _periodstart TIMESTAMP; _periodend TIMESTAMP; _cancelatend BOOLEAN;
BEGIN
    SELECT cs."PlanId", cs."StripeSubscriptionId", cs."CurrentPeriodStart", cs."CurrentPeriodEnd", cs."CancelAtPeriodEnd"
    INTO _activeplanid, _currentsubid, _periodstart, _periodend, _cancelatend
    FROM "CustomerSubscriptions" cs
    WHERE cs."CustomerId" = _customerid AND cs."SubscriptionStatus" = 'active' AND cs."CurrentPeriodEnd" >= CURRENT_TIMESTAMP
    ORDER BY cs."CurrentPeriodEnd" DESC LIMIT 1;

    RETURN QUERY
    SELECT P."PlanId", P."PlanName", P."StripeProductId", P."PlanDescription",
        PS."StripePriceId", PS."Amount", PS."Currency", PS."BillingInterval",
        (SELECT json_agg(json_build_object('BenefitKey', PB."BenefitKey", 'BenefitValue', PB."BenefitValue"))
         FROM "PlanBenefits" PB WHERE PB."PlanId" = P."PlanId"),
        _activeplanid, _currentsubid, _periodstart, _periodend, _cancelatend,
        CASE WHEN P."PlanId" = _activeplanid THEN 1 ELSE 0 END
    FROM "Plans" P
    INNER JOIN "PlanPrices" PS ON P."PlanId" = PS."PlanId" AND PS."IsActive" = TRUE
    WHERE P."IsActive" = TRUE;
END;
$$ LANGUAGE plpgsql;

-- SP_SaveCustomerSubscription
CREATE OR REPLACE FUNCTION "SP_SaveCustomerSubscription"(
    _customerid INT, _planid INT, _stripesubscriptionid VARCHAR(100),
    _subscriptionstatus VARCHAR(50), _currentperiodstart TIMESTAMP DEFAULT NULL,
    _currentperiodend TIMESTAMP DEFAULT NULL, _cancelatperiodend BOOLEAN DEFAULT FALSE
)
RETURNS VOID AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM "CustomerSubscriptions" WHERE "StripeSubscriptionId" = _stripesubscriptionid) THEN
        UPDATE "CustomerSubscriptions" SET "CustomerId"=_customerid,"PlanId"=_planid,
            "SubscriptionStatus"=_subscriptionstatus,"CurrentPeriodStart"=_currentperiodstart,
            "CurrentPeriodEnd"=_currentperiodend,"CancelAtPeriodEnd"=_cancelatperiodend,"UpdatedAt"=CURRENT_TIMESTAMP
        WHERE "StripeSubscriptionId" = _stripesubscriptionid;
    ELSE
        UPDATE "CustomerSubscriptions" SET "SubscriptionStatus"='canceled',"CancelAtPeriodEnd"=FALSE,"UpdatedAt"=CURRENT_TIMESTAMP
        WHERE "CustomerId" = _customerid AND "StripeSubscriptionId" <> _stripesubscriptionid
          AND "SubscriptionStatus" IN ('active','trialing');

        INSERT INTO "CustomerSubscriptions"("CustomerId","PlanId","StripeSubscriptionId","SubscriptionStatus",
            "CurrentPeriodStart","CurrentPeriodEnd","CancelAtPeriodEnd","CreatedAt")
        VALUES(_customerid,_planid,_stripesubscriptionid,_subscriptionstatus,
            _currentperiodstart,_currentperiodend,_cancelatperiodend,CURRENT_TIMESTAMP);
    END IF;

    IF _subscriptionstatus = 'canceled' THEN
        UPDATE "CustomerSubscriptions" SET "CancelAtPeriodEnd"=FALSE,"UpdatedAt"=CURRENT_TIMESTAMP
        WHERE "StripeSubscriptionId" = _stripesubscriptionid;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- SP_CancelSubscription
CREATE OR REPLACE FUNCTION "SP_CancelSubscription"(_stripesubscriptionid TEXT)
RETURNS TABLE(result INT) AS $$
BEGIN
    UPDATE "CustomerSubscriptions" SET "SubscriptionStatus" = 'canceled' WHERE "StripeSubscriptionId" = _stripesubscriptionid;
    RETURN QUERY SELECT 1;
END;
$$ LANGUAGE plpgsql;

-- SP_CheckSubscriptionAction
CREATE OR REPLACE FUNCTION "SP_CheckSubscriptionAction"(_customerid BIGINT, _requestedplanid INT)
RETURNS TABLE(
    "CustomerId" BIGINT, "CurrentPlanId" INT, "RequestedPlanId" INT,
    "CurrentPlanAmount" DECIMAL(18,2), "RequestedPlanAmount" DECIMAL(18,2),
    "SubscriptionStatus" VARCHAR(50), "CurrentPeriodEnd" TIMESTAMP, "ActionType" VARCHAR(50)
) AS $$
DECLARE
    _currentplanid INT; _currentamount DECIMAL(18,2); _requestedamount DECIMAL(18,2);
    _substatus VARCHAR(50); _periodend TIMESTAMP; _result VARCHAR(50);
BEGIN
    SELECT cs."PlanId", cs."SubscriptionStatus", cs."CurrentPeriodEnd"
    INTO _currentplanid, _substatus, _periodend
    FROM "CustomerSubscriptions" cs
    WHERE cs."CustomerId" = _customerid AND cs."SubscriptionStatus" = 'active'
      AND (cs."CancelAtPeriodEnd" = FALSE OR cs."CurrentPeriodEnd" >= CURRENT_TIMESTAMP)
    ORDER BY cs."CreatedAt" DESC LIMIT 1;

    SELECT pp."Amount" INTO _requestedamount FROM "PlanPrices" pp WHERE pp."PlanId" = _requestedplanid AND pp."IsActive" = TRUE LIMIT 1;

    IF _currentplanid IS NULL THEN
        _result := 'ALLOW_BUY';
    ELSE
        SELECT pp."Amount" INTO _currentamount FROM "PlanPrices" pp WHERE pp."PlanId" = _currentplanid AND pp."IsActive" = TRUE LIMIT 1;
        IF _requestedamount = _currentamount THEN _result := 'BLOCK_ALREADY_ACTIVE';
        ELSIF _requestedamount > _currentamount THEN _result := 'ALLOW_UPGRADE';
        ELSE _result := 'ALLOW_DOWNGRADE';
        END IF;
    END IF;

    RETURN QUERY SELECT _customerid, _currentplanid, _requestedplanid, _currentamount, _requestedamount, _substatus, _periodend, _result;
END;
$$ LANGUAGE plpgsql;

-- SP_GetPlanSubscriptonHistory (keeping original name)
CREATE OR REPLACE FUNCTION "SP_GetPlanSubscriptonHistory"(_planid BIGINT)
RETURNS TABLE(
    "PlanName" VARCHAR(100), "PlanDescription" VARCHAR(500), "SubscriptionStatus" VARCHAR(50),
    "CurrentPeriodStart" TIMESTAMP, "CurrentPeriodEnd" TIMESTAMP, "CancelAtPeriodEnd" BOOLEAN,
    "CreatedAt" TIMESTAMP, "FullName" TEXT, "Email" TEXT, "Benefits" JSON,
    "Amount" DECIMAL(10,2), "Currency" VARCHAR(10)
) AS $$
BEGIN
    RETURN QUERY
    SELECT P."PlanName", P."PlanDescription", CS."SubscriptionStatus",
        CS."CurrentPeriodStart", CS."CurrentPeriodEnd", CS."CancelAtPeriodEnd",
        CS."CreatedAt", C."FirstName" || C."LastName", C."Email",
        (SELECT json_agg(json_build_object('BenefitKey', PB."BenefitKey", 'BenefitValue', PB."BenefitValue"))
         FROM "PlanBenefits" PB WHERE PB."PlanId" = P."PlanId"),
        PP."Amount", PP."Currency"
    FROM "Plans" P
    INNER JOIN "CustomerSubscriptions" CS ON P."PlanId" = CS."PlanId"
    INNER JOIN "Customers" C ON CS."CustomerId" = C."CustomerId"
    INNER JOIN "PlanPrices" PP ON P."PlanId" = PP."PlanId"
    WHERE P."PlanId" = _planid;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- INACTIVE CART / NOTIFICATION FUNCTIONS
-- =====================================================

-- SP_GetInactiveCartUsers
CREATE OR REPLACE FUNCTION "SP_GetInactiveCartUsers"(_minutes INT DEFAULT 5)
RETURNS TABLE(
    "CustomerId" BIGINT, "FirstName" TEXT, "LastName" TEXT, "Email" TEXT,
    "FCMToken" TEXT, "CartId" BIGINT, "UpdatedAt" TIMESTAMP,
    "TotalItems" BIGINT, "TotalQuantity" BIGINT, "CartValue" DECIMAL,
    "LastAttempt" INT, "LastSentAt" TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    WITH FilteredCart AS (
        SELECT PC.*, ROW_NUMBER() OVER (PARTITION BY PC."CustomerId" ORDER BY PC."UpdatedAt" DESC) AS rn
        FROM "ProductCart" PC
        WHERE PC."Status" = 0 AND EXISTS (SELECT 1 FROM "ProductCartItem" PCI WHERE PCI."CartSessionId" = PC."Id" AND PCI."Active" = TRUE)
    ),
    LatestLog AS (
        SELECT cnl."CartId", MAX(cnl."AttemptNumber") AS "LastAttempt", MAX(cnl."SentAt") AS "LastSentAt"
        FROM "CartNotificationLog" cnl WHERE cnl."NotificationType" = 'InactiveCartNotify'
        GROUP BY cnl."CartId"
    )
    SELECT C."CustomerId", C."FirstName", C."LastName", C."Email", C."FCMToken",
        PC2."Id", PC2."UpdatedAt", COUNT(PCI2."Id"), SUM(PCI2."Quantity")::BIGINT,
        SUM(PCI2."Price" * PCI2."Quantity"), COALESCE(LL."LastAttempt", 0)::INT, LL."LastSentAt"
    FROM "Customers" C
    INNER JOIN FilteredCart PC2 ON PC2."CustomerId" = C."CustomerId" AND PC2.rn = 1
    INNER JOIN "ProductCartItem" PCI2 ON PCI2."CartSessionId" = PC2."Id"
    LEFT JOIN LatestLog LL ON LL."CartId" = PC2."Id"
    WHERE C."IsDeleted" = FALSE AND C."FCMToken" IS NOT NULL AND PCI2."Active" = TRUE
      AND PC2."UpdatedAt" < NOW() - (_minutes || ' minutes')::INTERVAL
      AND NOT EXISTS (SELECT 1 FROM "Orders" O WHERE O."CartSessionId" = PC2."SessionId")
      AND COALESCE(LL."LastAttempt", 0) < 3
      AND (LL."LastSentAt" IS NULL
           OR (COALESCE(LL."LastAttempt",0) = 1 AND LL."LastSentAt" < NOW() - INTERVAL '30 minutes')
           OR (COALESCE(LL."LastAttempt",0) = 2 AND LL."LastSentAt" < NOW() - INTERVAL '120 minutes'))
    GROUP BY C."CustomerId", C."FirstName", C."LastName", C."Email", C."FCMToken",
        PC2."Id", PC2."UpdatedAt", LL."LastAttempt", LL."LastSentAt";
END;
$$ LANGUAGE plpgsql;

-- SP_InsertCartNotificationLog
CREATE OR REPLACE FUNCTION "SP_InsertCartNotificationLog"(
    _cartid INT, _customerid INT, _notificationtype VARCHAR(50) DEFAULT 'AbandonedCart'
)
RETURNS TABLE(result BIGINT) AS $$
DECLARE _attemptnum INT; _newid BIGINT;
BEGIN
    SELECT COALESCE(MAX("AttemptNumber"), 0) + 1 INTO _attemptnum
    FROM "CartNotificationLog" WHERE "CartId" = _cartid;

    INSERT INTO "CartNotificationLog"("CartId","CustomerId","NotificationType","SentAt","AttemptNumber")
    VALUES(_cartid, _customerid, _notificationtype, NOW(), _attemptnum)
    RETURNING "Id" INTO _newid;
    RETURN QUERY SELECT _newid::BIGINT;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- END OF MIGRATION SCRIPT
-- =====================================================
-- After running this script, your neondb will have:
-- ✅ 35 business tables (HangFire tables excluded - use Hangfire.PostgreSql NuGet)
-- ✅ 70+ PostgreSQL functions matching your C# SP names
-- ✅ All indexes recreated
-- ✅ All defaults and constraints preserved
-- =====================================================


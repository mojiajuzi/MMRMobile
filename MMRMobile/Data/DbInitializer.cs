using Microsoft.EntityFrameworkCore;

namespace MMRMobile.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        // 创建表结构
        context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Contacts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Phone TEXT,
                Email TEXT,
                Wechat TEXT,
                DateCreated TEXT NOT NULL,
                DateModified TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Tags (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Active INTEGER NOT NULL DEFAULT 1,
                DateCreated TEXT NOT NULL,
                DateModified TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS ContactTags (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ContactId INTEGER NOT NULL,
                TagId INTEGER NOT NULL,
                FOREIGN KEY (ContactId) REFERENCES Contacts(Id),
                FOREIGN KEY (TagId) REFERENCES Tags(Id)
            );

            CREATE TABLE IF NOT EXISTS Works (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Status TEXT NOT NULL,
                Funds REAL NOT NULL DEFAULT 0,
                StartAt TEXT,
                EndAt TEXT,
                DateCreated TEXT NOT NULL,
                DateModified TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS WorkTags (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                WorkId INTEGER NOT NULL,
                TagId INTEGER NOT NULL,
                FOREIGN KEY (WorkId) REFERENCES Works(Id),
                FOREIGN KEY (TagId) REFERENCES Tags(Id)
            );

            CREATE TABLE IF NOT EXISTS WorkContacts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                WorkId INTEGER NOT NULL,
                ContactId INTEGER NOT NULL,
                Amount REAL NOT NULL DEFAULT 0,
                IsCome INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (WorkId) REFERENCES Works(Id),
                FOREIGN KEY (ContactId) REFERENCES Contacts(Id)
            );

            CREATE TABLE IF NOT EXISTS WorkPayments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                WorkId INTEGER NOT NULL,
                ContactId INTEGER NOT NULL,
                Amount REAL NOT NULL DEFAULT 0,
                IsIncome INTEGER NOT NULL DEFAULT 0,
                HasInvoice INTEGER NOT NULL DEFAULT 0,
                PaymentDate TEXT NOT NULL,
                Remark TEXT,
                DateCreated TEXT NOT NULL,
                DateModified TEXT NOT NULL,
                FOREIGN KEY (WorkId) REFERENCES Works(Id),
                FOREIGN KEY (ContactId) REFERENCES Contacts(Id)
            );
        ");
    }
} 
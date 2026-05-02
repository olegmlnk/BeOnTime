using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Settings;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Tests
{
    public class UserSettingsServiceTests
    {
        [Fact]
        public async Task GetAsync_ShouldReturnExistingSettings()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var settings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Theme = UiTheme.Dark,
                Language = Language.En,
                TimeZone = "Europe/Kyiv",
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(settings);

            // 2. ACT
            var result = await service.GetAsync(userId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal(UiTheme.Dark, result.Theme);
            Assert.Equal(Language.En, result.Language);
            Assert.Equal("Europe/Kyiv", result.TimeZone);
        }

        [Fact]
        public async Task GetAsync_ShouldCreateDefaults_WhenSettingsNotFound()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();

            // Налаштувань немає — повертаємо null
            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync((UserSettings?)null);

            // Мокаємо CreateAsync, щоб він повертав збережений об'єкт
            mockSettings
                .Setup(r => r.CreateAsync(It.IsAny<UserSettings>()))
                .ReturnsAsync((UserSettings s) => s);

            // 2. ACT
            var result = await service.GetAsync(userId);

            // 3. ASSERT
            Assert.NotNull(result);
            // Перевірка, що CreateAsync викликано (створено дефолтні налаштування)
            mockSettings.Verify(r => r.CreateAsync(It.IsAny<UserSettings>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSaveSnapshotAndApplyChanges()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var existingSettings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Theme = UiTheme.Light,
                Language = Language.Uk,
                TimeZone = "Europe/Kyiv",
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingSettings);

            var dto = new UpdateUserSettingsDto
            {
                Theme = UiTheme.Dark,
                Language = Language.En,
                TimeZone = "America/New_York",
                UpcomingHorizonDays = 14,
                RemindersPollIntervalSeconds = 30
            };

            // 2. ACT
            var result = await service.UpdateAsync(userId, dto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal(UiTheme.Dark, result.Theme);
            Assert.Equal(Language.En, result.Language);
            Assert.Equal("America/New_York", result.TimeZone);

            // Перевірка, що старий стан зберігся в історію
            mockHistory.Verify(r => r.CreateAsync(It.IsAny<UserSettingsHistory>()), Times.Once);

            // Перевірка, що зміни збережені
            mockSettings.Verify(r => r.UpdateAsync(existingSettings), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldKeepOldTimeZone_WhenNewTimeZoneIsEmpty()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var existingSettings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TimeZone = "Europe/Kyiv",
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingSettings);

            var dto = new UpdateUserSettingsDto
            {
                TimeZone = "", // Пустий — має залишитися старий
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            // 2. ACT
            var result = await service.UpdateAsync(userId, dto);

            // 3. ASSERT
            Assert.Equal("Europe/Kyiv", result.TimeZone);
        }

        [Fact]
        public async Task UpdateAsync_ShouldClampNegativeReminderLeadMinutes()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var existingSettings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingSettings);

            var dto = new UpdateUserSettingsDto
            {
                DefaultReminderLeadMinutes = -5, // Від'ємне — має стати 0
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            // 2. ACT
            var result = await service.UpdateAsync(userId, dto);

            // 3. ASSERT
            Assert.Equal(0, result.DefaultReminderLeadMinutes);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUse7Days_WhenUpcomingHorizonDaysLessThan1()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var existingSettings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingSettings);

            var dto = new UpdateUserSettingsDto
            {
                UpcomingHorizonDays = 0, // Менше за 1 — має стати 7
                RemindersPollIntervalSeconds = 60
            };

            // 2. ACT
            var result = await service.UpdateAsync(userId, dto);

            // 3. ASSERT
            Assert.Equal(7, result.UpcomingHorizonDays);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUse60Seconds_WhenPollIntervalLessThan15()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var existingSettings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingSettings);

            var dto = new UpdateUserSettingsDto
            {
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 5 // Менше за 15 — має стати 60
            };

            // 2. ACT
            var result = await service.UpdateAsync(userId, dto);

            // 3. ASSERT
            Assert.Equal(60, result.RemindersPollIntervalSeconds);
        }

        [Fact]
        public async Task GetHistoryAsync_ShouldReturnMappedHistory()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var historyItems = new List<UserSettingsHistory>
            {
                new UserSettingsHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Theme = UiTheme.Light,
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new UserSettingsHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Theme = UiTheme.Dark,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                }
            };

            mockHistory
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(historyItems);

            // 2. ACT
            var result = await service.GetHistoryAsync(userId);

            // 3. ASSERT
            Assert.Equal(2, result.Count);
            Assert.Equal(UiTheme.Light, result[0].Theme);
            Assert.Equal(UiTheme.Dark, result[1].Theme);
        }

        [Fact]
        public async Task RestoreFromHistoryAsync_ShouldReturnFail_WhenSnapshotNotFound()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();

            mockHistory
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((UserSettingsHistory?)null);

            // 2. ACT
            var result = await service.RestoreFromHistoryAsync(userId, Guid.NewGuid());

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("History entry not found", result.Error);
        }

        [Fact]
        public async Task RestoreFromHistoryAsync_ShouldRestoreSettingsFromSnapshot()
        {
            // 1. ARRANGE
            var mockSettings = new Mock<IUserSettingsRepository>();
            var mockHistory = new Mock<IUserSettingsHistoryRepository>();
            var service = new UserSettingsService(mockSettings.Object, mockHistory.Object);

            var userId = Guid.NewGuid();
            var historyId = Guid.NewGuid();

            // Знімок зі старими налаштуваннями
            var snapshot = new UserSettingsHistory
            {
                Id = historyId,
                UserId = userId,
                Theme = UiTheme.Light,
                Language = Language.En,
                TimeZone = "America/Chicago",
                UpcomingHorizonDays = 14,
                RemindersPollIntervalSeconds = 30
            };

            // Поточні налаштування
            var currentSettings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Theme = UiTheme.Dark,
                Language = Language.Uk,
                TimeZone = "Europe/Kyiv",
                UpcomingHorizonDays = 7,
                RemindersPollIntervalSeconds = 60
            };

            mockHistory
                .Setup(r => r.GetByIdAsync(userId, historyId))
                .ReturnsAsync(snapshot);
            mockSettings
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(currentSettings);

            // 2. ACT
            var result = await service.RestoreFromHistoryAsync(userId, historyId);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.NotNull(result.Value);

            // Перевірка, що налаштування відновлено з snapshot
            Assert.Equal(UiTheme.Light, currentSettings.Theme);
            Assert.Equal(Language.En, currentSettings.Language);
            Assert.Equal("America/Chicago", currentSettings.TimeZone);
            Assert.Equal(14, currentSettings.UpcomingHorizonDays);

            // Перевірка, що перед відновленням зберігся знімок поточного стану
            mockHistory.Verify(r => r.CreateAsync(It.IsAny<UserSettingsHistory>()), Times.Once);
            mockSettings.Verify(r => r.UpdateAsync(currentSettings), Times.Once);
        }
    }
}

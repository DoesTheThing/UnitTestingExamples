using System.Reflection;
using UnitTestEx;
using Assert = NUnit.Framework.Assert;
using File = UnitTestEx.File;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for FileStorageTest
    /// </summary>
    [TestFixture]
    public class FileStorageTest
    {
        public const string MAX_SIZE_EXCEPTION = "DIFFERENT MAX SIZE";
        public const string NULL_FILE_EXCEPTION = "NULL FILE";
        public const string NO_EXPECTED_EXCEPTION_EXCEPTION = "There is no expected exception";

        public const string SPACE_STRING = " ";
        public const string FILE_PATH_STRING = "@D:\\JDK-intellij-downloader-info.txt";
        public const string CONTENT_STRING = "Some text";
        public const string REPEATED_STRING = "AA";
        public const string WRONG_SIZE_CONTENT_STRING = "TEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtext";
        public const string TIC_TOC_TOE_STRING = "tictoctoe.game";

        public const int STORAGE_SIZE = 5;

        public FileStorage storage = new FileStorage(STORAGE_SIZE);

        /* ПРОВАЙДЕРЫ */

        static object[] NewFilesData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) },
            new object[] { new File(SPACE_STRING, WRONG_SIZE_CONTENT_STRING) },
            new object[] { new File(FILE_PATH_STRING, CONTENT_STRING) }
        };

        static object[] FilesForDeleteData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING), REPEATED_STRING },
            new object[] { null!, TIC_TOC_TOE_STRING }
        };

        static object[] NewExceptionFileData = {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) }
        };

        static object[] FileArraysForDeletingAll =
        {
            new object[] {new File[] { new File(REPEATED_STRING, CONTENT_STRING), new File(FILE_PATH_STRING, CONTENT_STRING) } },
            new object[] { new File[] { new File(REPEATED_STRING, CONTENT_STRING) } },
        };

        /// <summary>
        /// Test deleting all files
        /// </summary>
        [Test, TestCaseSource(nameof(FileArraysForDeletingAll))]
        public void DeleteAllFilesTest(params File[] files)
        {
            foreach (File file in files)
                storage.Write(file);

            Assert.That(storage.DeleteAllFiles());

            foreach (File file in files)
                Assert.That(storage.IsExists(file.GetFilename()), Is.False);

            Assert.That(storage.GetFiles().Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteAllFilesTestWithEmptyStorage()
        {
            Assert.DoesNotThrow(() => storage.DeleteAllFiles());
            Assert.That(storage.GetFiles().Count, Is.EqualTo(0));
        }

        [Test, TestCaseSource(nameof(FileArraysForDeletingAll))]
        public void GetFilesTest(params File[] files)
        {
            foreach (File file in files)
                storage.Write(file);

            Assert.That(storage.GetFiles(), Is.EquivalentTo(files));
        }

        /* Тестирование записи файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void WriteTest(File file)
        {
            if (file.GetSize() > STORAGE_SIZE)
                Assert.False(storage.Write(file));
            else
                Assert.True(storage.Write(file));
        }

        /* Тестирование записи дублирующегося файла */
        [Test, TestCaseSource(nameof(NewExceptionFileData))]
        public void WriteExceptionTest(File file)
        {
            storage.Write(file);

            Assert.Throws<FileNameAlreadyExistsException>(() => storage.Write(file), NO_EXPECTED_EXCEPTION_EXCEPTION);
        }

        /* Тестирование проверки существования файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void IsExistsTest(File file)
        {
            bool isWritten = false;
            string name = file.GetFilename();
            Assert.False(storage.IsExists(name));

            try
            {
                isWritten = storage.Write(file);
            }
            catch (FileNameAlreadyExistsException e)
            {
                Console.WriteLine(String.Format("Exception {0} in method {1}", e.GetBaseException(), MethodBase.GetCurrentMethod()?.Name));
            }

            if (isWritten)
                Assert.True(storage.IsExists(name));
            else
                Assert.False(storage.IsExists(name));
        }

        /* Тестирование удаления файла */
        [Test, TestCaseSource(nameof(FilesForDeleteData))]
        public void DeleteTest(File file, String fileName)
        {
            try
            {
                storage.Write(file);
                Assert.True(storage.Delete(fileName));
            }
            catch (NullReferenceException)
            {
                Assert.False(storage.Delete(fileName));
            }
        }

        // Почти эталонный
        /* Тестирование получения файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void GetFileTest(File expectedFile)
        {
            if (!storage.Write(expectedFile))
                Assert.Pass();

            File actualfile = storage.GetFile(expectedFile.GetFilename());
            bool difference = !actualfile.GetFilename().Equals(expectedFile.GetFilename()) || !actualfile.GetSize().Equals(expectedFile.GetSize());

            Assert.IsFalse(difference, string.Format("There is some differences in {0} or {1}", expectedFile.GetFilename(), expectedFile.GetSize()));
        }

        [TearDown]
        public void CleanUp()
        {
            if (storage.GetFiles().Count > 0)
                storage.DeleteAllFiles();
        }
    }
}

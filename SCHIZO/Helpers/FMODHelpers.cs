using FMOD;
using FMODUnity;
using SCHIZO.Resources;

namespace SCHIZO.Helpers;
internal static class FMODHelpers
{
    /// <summary>
    /// Attempts to load an FMOD bank from embedded resources.
    /// </summary>
    /// <param name="fileName">The filename of the bank - e.g. "Mod.bank".</param>
    /// <param name="throwOnFail">Throw a <see cref="BankLoadException"/> if the load did not succeed.</param>
    /// <exception cref="BankLoadException">Thrown with the result of the attempted load if <paramref name="throwOnFail"/> is set.</exception>
    public static void LoadBankFromResources(string fileName, bool throwOnFail = false)
    {
        byte[] fmodBank = ResourceManager.GetEmbeddedBytes(fileName, throwOnFail);
        if (fmodBank == null) return; // already thrown above if throwIfMissing is set

        RESULT res = RuntimeManager.StudioSystem.loadBankMemory(fmodBank, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out FMOD.Studio.Bank bank);
        res.CheckResult();
        if (!bank.hasHandle() && throwOnFail)
            throw new BankLoadException(fileName, res);
    }

    public static void LoadMasterBank(string name, bool throwOnFail = false)
    {
        LoadBankFromResources($"{name}.bank", throwOnFail);
        LoadBankFromResources($"{name}.strings.bank", throwOnFail);
    }

    public static void LoadSubBank(string name, bool throwOnFail = false)
    {
        LoadBankFromResources($"{name}.bank", throwOnFail);
    }
}

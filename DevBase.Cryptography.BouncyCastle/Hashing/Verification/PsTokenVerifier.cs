﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;

namespace DevBase.Cryptography.BouncyCastle.Hashing.Verification;

public class PsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
    {
        IDigest digest = (IDigest)Activator.CreateInstance(typeof(T))!;

        using StringReader stringReader = new StringReader(publicKey);

        PemReader pemReader = new PemReader(stringReader);
        AsymmetricKeyParameter asymmetricKeyParameter = (AsymmetricKeyParameter)pemReader.ReadObject();
        
        stringReader.Close();

        PssSigner signer = new PssSigner(new RsaBlindedEngine(), digest);
        signer.Init(false, asymmetricKeyParameter);
        signer.BlockUpdate(content, 0, content.Length);
        
        return signer.VerifySignature(signature);
    }
}
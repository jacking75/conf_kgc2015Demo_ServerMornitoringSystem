
public static class Util
{
  // 보안 코드를 만든다. 휘발성 인증코드 생성 등에 사용한다.
  // 출처: http://stackoverflow.com/questions/54991/generating-random-passwords
  public static string GenerateSecureString(int lowercase, int uppercase, int numerics)
  {
      string lowers = "abcdefghijklmnopqrstuvwxyz";
      string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      string number = "0123456789";

      Random random = new Random();

      string generated = "!";
      for (int i = 1; i <= lowercase; i++)
          generated = generated.Insert(
              random.Next(generated.Length),
              lowers[random.Next(lowers.Length - 1)].ToString()
          );

      for (int i = 1; i <= uppercase; i++)
          generated = generated.Insert(
              random.Next(generated.Length),
              uppers[random.Next(uppers.Length - 1)].ToString()
          );

      for (int i = 1; i <= numerics; i++)
          generated = generated.Insert(
              random.Next(generated.Length),
              number[random.Next(number.Length - 1)].ToString()
          );

      return generated.Replace("!", string.Empty);
  }

}

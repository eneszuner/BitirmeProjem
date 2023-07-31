using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect : MethodInterception //Aspect
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            //defensive coding
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Bu bir doğrulama sınıfı değil");
            }

            _validatorType = validatorType;
        }
        protected override void OnBefore(IInvocation invocation)
        {
            var validator = (IValidator)Activator.CreateInstance(_validatorType);//reflecsion biz birşeyi newlerken ama newleme işni çalışma anında yapmak istiyorsak
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];//Çalışma tipini bul diyor burada
            var entities = invocation.Arguments.Where(t => t.GetType() == entityType);//bu sefer parametrelerini bul demek ilgili metotun parametrelerini bul 
            /*metotdun parametrelerine bak entityType Producta denk gelen kısaca diyorki validator'un tipine eşit olan 
             * parametreleri git bul diyor yani ProductManager daki Add Product'ı git bul
            invocation metot demek
             */

            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
        }
    }
}

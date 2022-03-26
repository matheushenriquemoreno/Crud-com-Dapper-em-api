using APIeCommerceCursoDapper.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace APIeCommerceCursoDapper.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorio<Usuario>
    {
        private IDbConnection _conecao;

        public UsuarioRepositorio()
        {
            _conecao = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CrusoDapper;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        // ADO.Net -> Dapper: Micro-ORM (MER <--> POO)

        public List<Usuario> Get()
        {

            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine(" SELECT  U.*, C.*, ENT.*, D.* FROM Usuarios U ");
            consulta.AppendLine(" LEFT JOIN Contatos C on C.UsuarioId = U.Id ");
            consulta.AppendLine(" LEFT JOIN EnderecosEntrega ENT on ENT.UsuarioId = U.id ");
            consulta.AppendLine(" LEFT JOIN UsuariosDepartamentos UD on UD.UsuarioId = U.Id ");
            consulta.AppendLine(" LEFT JOIN Departamentos D on D.Id = UD.DepartamentoId ");

            var listaUsuarios = new List<Usuario>();

            _conecao.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(consulta.ToString(),
                (usuario, contato, enderecoEntrega, departamento) => // funcao executada 1 vez por linha do select 
                {
                    //verifico se o usuario ja existe na lista se não adiciono;
                    if (!listaUsuarios.Any(x => x.Id == usuario.Id))
                    {
                        usuario.Contato = contato;
                        listaUsuarios.Add(usuario);
                    }

                    // reescrevo o usuario para o da lista.
                    usuario = listaUsuarios.Where(x => x.Id == usuario.Id).FirstOrDefault();

                    // verifico se o endereco de entrega ja existe se não existir adiciono.
                    if (!usuario.EnderecosEntrega.Any(x => x.Id == enderecoEntrega.Id))
                    {
                        usuario.AdicionarEnderecoEntrega(enderecoEntrega);
                    }
                    // verifico se o Departamento ja existe se não existir adiciono.
                    if (!usuario.Departamentos.Any(x => x.Id == departamento.Id))
                    {
                        usuario.AdicionarDepartamentos(departamento);
                    }

                    return null; //esse retorno não e importante.
                }).ToList();

            return listaUsuarios;


            // pra buscar dessa forma o nome das colunas da tabela tem que ser igual as propriedadades do usuario 

        }

        public Usuario Get(int id)
        {
            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine(" SELECT  U.*, C.*, ENT.*, D.* FROM Usuarios U ");
            consulta.AppendLine(" LEFT JOIN Contatos C on C.UsuarioId = U.Id ");
            consulta.AppendLine(" LEFT JOIN EnderecosEntrega ENT on ENT.UsuarioId = U.id ");
            consulta.AppendLine(" LEFT JOIN UsuariosDepartamentos UD on UD.UsuarioId = U.Id ");
            consulta.AppendLine(" LEFT JOIN Departamentos D on D.Id = UD.DepartamentoId ");
            consulta.AppendLine(" WHERE U.Id = @id");

            var usuarioBuscado = new Usuario();

            var usuario = _conecao.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(consulta.ToString(),
                  (usuario, contato, enderecoEntrega, departamento) =>
                  {
                      if (usuarioBuscado.Id != usuario.Id)
                      {
                          usuarioBuscado = usuario;
                          usuarioBuscado.Contato = contato;
                      }

                      if (!usuarioBuscado.EnderecosEntrega.Any(x => x.Id == enderecoEntrega.Id))
                      {
                          usuarioBuscado.AdicionarEnderecoEntrega(enderecoEntrega);
                      }

                      if (!usuarioBuscado.Departamentos.Any(x => x.Id == departamento.Id))
                      {
                          usuarioBuscado.AdicionarDepartamentos(departamento);
                      }

                      return null;
                  },
                  new { Id = id }).ToList();

            return usuarioBuscado;

        }

        public void Cadastrar(Usuario usuarioInsercao)
        {

            _conecao.Open();

            var transaction = _conecao.BeginTransaction(); // criado pra evitar problemas de inserção no banco.

            try
            {
                StringBuilder queryInsert = new StringBuilder();

                queryInsert.AppendLine("INSERT INTO usuarios(Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro)");
                queryInsert.AppendLine(" VALUES");
                queryInsert.AppendLine("(@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro);");
                queryInsert.AppendLine("SELECT CAST(SCOPE_IDENTITY() AS INT);");

                usuarioInsercao.Id = _conecao.Query<int>(queryInsert.ToString(), usuarioInsercao, transaction).Single(); // executa a querry e com base na inserção retorna o id inserido

                if (usuarioInsercao.Contato != null)
                {
                    usuarioInsercao.Contato.UsuarioId = usuarioInsercao.Id;
                    
                    StringBuilder insertContato = new StringBuilder();

                    insertContato.AppendLine("INSERT INTO Contatos(UsuarioId, Telefone, Celular)");
                    insertContato.AppendLine("VALUES");
                    insertContato.AppendLine("(@UsuarioId, @Telefone, @Celular);");
                    insertContato.AppendLine("SELECT CAST(SCOPE_IDENTITY() AS INT);");
     
                    usuarioInsercao.Contato.Id = _conecao.Query<int>(insertContato.ToString(), usuarioInsercao.Contato, transaction).Single();
                }

                if (usuarioInsercao.EnderecosEntrega.Count > 0)
                {
                    foreach (var endereco in usuarioInsercao.EnderecosEntrega)
                    {

                        endereco.UsuarioID = usuarioInsercao.Id;

                        StringBuilder insertEndereco = new StringBuilder();

                        insertEndereco.AppendLine("INSERT INTO EnderecosEntrega(UsuarioID, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) ");
                        insertEndereco.AppendLine("VALUES (@UsuarioID, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento);");
                        insertEndereco.AppendLine("SELECT CAST(SCOPE_IDENTITY() AS INT);");

                        endereco.Id = _conecao.Query<int>(insertEndereco.ToString(), endereco, transaction).First();
                    }
                }

                if (usuarioInsercao.Departamentos.Count > 0)
                {
                    foreach (var departamento in usuarioInsercao.Departamentos)
                    {

                        string InsertUsuariosDepartamentos = "INSERT INTO UsuariosDepartamentos(UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                        _conecao.Execute(InsertUsuariosDepartamentos, new { UsuarioId = usuarioInsercao.Id, DepartamentoId = departamento.Id}, transaction);
                    }
                }

                transaction.Commit(); // aprova as transações no banco se não tiver nenhum erro 

            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback(); // cancela a transação caso de algum erro 
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException("Erro ao cadastrar! " + ex.Message);
                }

            }
            finally
            {
                _conecao.Close();
            }
        }

        public void Update(Usuario usuarioUpdate)
        {
            _conecao.Open();

            var transaction = _conecao.BeginTransaction();

            try
            {
                StringBuilder queryUpdate = new StringBuilder();

                queryUpdate.AppendLine("UPDATE usuarios");
                queryUpdate.AppendLine(" SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG  = @RG, CPF  = @CPF, NomeMae  = @NomeMae, SituacaoCadastro  = @SituacaoCadastro, DataCadastro = @DataCadastro");
                queryUpdate.AppendLine(" WHERE Id = @id");
     
                _conecao.Execute(queryUpdate.ToString(), usuarioUpdate, transaction);

                if (usuarioUpdate.Contato != null)
                {
                    string comandoUpdateContato = "UPDATE contatos SET Telefone = @Telefone, Celular = @Celular where id = @id";
                    _conecao.Execute(comandoUpdateContato, usuarioUpdate.Contato, transaction);
                }

                if (usuarioUpdate.EnderecosEntrega.Count > 0)
                {
                    foreach (var endereco in usuarioUpdate.EnderecosEntrega)
                    {
                        endereco.UsuarioID = usuarioUpdate.Id;

                        StringBuilder updateIndereco = new StringBuilder();

                        updateIndereco.AppendLine("UPDATE EnderecosEntrega ");
                        updateIndereco.AppendLine("SET NomeEndereco = @NomeEndereco, CEP = @CEP, Estado = @Estado, Cidade = @Cidade, Bairro = @Bairro, Endereco = @Endereco, Complemento = @Complemento");
                        updateIndereco.AppendLine("WHERE id = @Id");

                       _conecao.Execute(updateIndereco.ToString(), endereco, transaction);
                    }
                }

                if (usuarioUpdate.Departamentos.Count > 0)
                {
                    foreach (var departamento in usuarioUpdate.Departamentos)
                    {
                        StringBuilder updateDepartamento = new StringBuilder();

                        updateDepartamento.AppendLine("UPDATE UsuariosDepartamentos");
                        updateDepartamento.AppendLine("SET UsuarioId = @UsuarioId, DepartamentoId = @DepartamentoId");
                        updateDepartamento.AppendLine("WHERE DepartamentoId = @Id");

                        _conecao.Execute(updateDepartamento.ToString(), new { UsuarioId = usuarioUpdate.Id, DepartamentoId = departamento.Id, Id = departamento.Id },  transaction);

                    }
                }

                transaction.Commit();

            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException("Erro ao Atualizar! " + ex.Message);
                }
            }
            finally
            {
                _conecao.Close();
            }

        }

        //  Observações do delete 
        //       
        //   O delete Depende muito de regra de negocio e como o banco de dados foi criado. na criação do banco que estou trabalhando foi inserido a verificação ON DELETE CASCATE 
        //   que faz com que quando o usuario Pai for excluido seus vinculos também serão.

        //   agora em um banco onde essa opção não foi inserida, e necessario excluir todos os registros filhos antes de deletar a informação principal. se não gera um erro de integridade referencial.
        //   Exemplo:
        //   pra deletar o usuario teria que deletar primeiro sua presença nas tabelas seguinto um sitema Hierárquico
        //   -> contatos -> departamentos -> EnderecosEntrega -> UsuariosDepartamentos

        public void Delete(int id)
        {
            _conecao.Execute("DELETE FROM usuarios WHERE Id = @id", new { Id = id });
        }
    }
}
